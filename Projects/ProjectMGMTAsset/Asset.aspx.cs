using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Security.Cryptography;

namespace Production
{
    public partial class Asset : System.Web.UI.Page
    {
        // Dynamic field definitions (including ColumnOrder).
        private List<DynamicField> dynamicFields;
        // This is not used for validation in this example so we return an empty JSON object.
        protected string LimitsJson = "{}";
        protected int assetId = 0;
        protected string assetName = "";
        protected int editState = 0;

        // To store the current maintenance record ID (from ACC_Maintenance) for this asset.
        protected int currentMaintenanceId = 0;
        string username;
        string UID;



        // OnInit: load dynamic fields, create entry controls, and build GridView columns (only once)
        protected override void OnInit(EventArgs e)
        {
            currentMaintenanceId = 0;
            base.OnInit(e);
            // Ensure the cookie exists.
            if (Request.Cookies["AssetCookie"] == null)
            {
                Response.Redirect("Default.aspx");
            }
            else
            {
                assetId = Convert.ToInt32(Request.Cookies["AssetCookie"]["AssetID"]);
                assetName = Request.Cookies["AssetCookie"]["AssetName"];
                lblAssetName.Text = assetName;
            }
            // Load dynamic fields from ACC_AssetInfo (including ColumnOrder).
            LoadDynamicFields();
            // Create dynamic entry controls in a table layout (up to 4 per row).
            CreateDynamicControls();
            BindGrid();
        }

        // Always bind the grid.
        protected void Page_Load(object sender, EventArgs e)
        {
            // Always bind the grid so that DataKeys are available during events.
            DateTime now = DateTime.Now;
            lblCurrentDate.Text = now.ToString("yyyy-MM-dd HH:mm:ss");
            lblCurrentShift.Text = GetCurrentShift(now);
            // Load expected numeric limits from ACC_Limits.
            LoadLimits();
            BindGrid();
            if (!IsPostBack)
            {
                // Retrieve the Windows username (e.g., DOMAIN\username)
                Session["UserName"] = User.Identity.Name;
                // Get the current user's identity.
                username = User.Identity.Name;  // e.g., "DOMAIN\ChemJohn" or "ChemJohn"

                // If there is a domain prefix (e.g., "DOMAIN\"), remove it.
                int backslashIndex = username.IndexOf("\\");
                if (backslashIndex >= 0)
                {
                    UID = username.Substring(backslashIndex + 1);
                }

                // Check if the username starts with "Chem" (case-insensitive).
                if (username.StartsWith("CHEMET", StringComparison.OrdinalIgnoreCase))
                {
                    // Copy the username without "CHEMET" into a local variable UID.
                    UID = username.Substring(7).Trim(); // Remove the first 4 characters.
                                                        // You can now use UID for further processing.
                                                        // For example, you might store it in a session variable:
                    Session["UID"] = UID;
                }
                else
                {
                    // User does not start with "CHEMET", so "boot" the user.
                    // For example, redirect them to an Access Denied page:
                    Response.Redirect("AccessDenied.aspx");
                }
                //      Session["DisplayName"] = username;
                lblWelcome.Text = UID;
                CheckMaintenanceStatus();
            }
        }

        /// <summary>
        /// Loads expected numeric limits from ACC_Limits for this asset (or “page”).
        /// Assumes ACC_Limits has columns: PageNumber, TextBoxName, LowerLimit, UpperLimit.
        /// </summary>
        private void LoadLimits()
        {
            Dictionary<string, Limit> limits = new Dictionary<string, Limit>();
            string connectionString = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                string query = "SELECT TextBoxName, TextBoxType, LowerLimitF, UpperLimitF FROM ACC_Limits WHERE TextBoxType in ('Float','Int') and AssetID = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("AssetID", OdbcType.Int).Value = assetId;
                conn.Open();
                OdbcDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string textboxName = reader["TextBoxName"].ToString();
                    string textboxType = reader["TextBoxType"].ToString();
                    double lower = Convert.ToDouble(reader["LowerLimitF"]);
                    double upper = Convert.ToDouble(reader["UpperLimitF"]);
                    limits[textboxName] = new Limit { TextboxType = textboxType, LowerLimitF = lower, UpperLimitF = upper };
                }
                conn.Close();
            }

            using (OdbcConnection conn = new OdbcConnection(connectionString))
            {
                string query = "SELECT TextBoxName, TextBoxType, LowerLimitT, UpperLimitT FROM ACC_Limits WHERE TextBoxType in ('Time') and AssetID = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("AssetID", OdbcType.Int).Value = assetId;
                conn.Open();
                OdbcDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string textboxName = reader["TextBoxName"].ToString();
                    string textboxType = reader["TextBoxType"].ToString();
                    DateTime lowerT = Convert.ToDateTime(reader["LowerLimitT"]);
                    DateTime upperT = Convert.ToDateTime(reader["UpperLimitT"]);
                    limits[textboxName] = new Limit { TextboxType = textboxType, LowerLimitT = lowerT, UpperLimitT = upperT };
                }
                conn.Close();
            }


            // Serialize the limits dictionary to JSON so it can be used in JavaScript.
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LimitsJson = serializer.Serialize(limits);
        }

        private void PopulateDropDown(DropDownList ddl, int dropDownID)
        {
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            DataTable dt = new DataTable();
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                // Assume ACC_DropDown has columns: ID, DropDownID, OrderId, DisplayValue.
                string query = "SELECT * FROM ACC_DropDown WHERE DropDownID = ? ORDER BY OrderId";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = dropDownID;
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(dt);
            }
            ddl.DataSource = dt;
            ddl.DataTextField = "DisplayValue";
            // Optionally, if there's a value column separate from DisplayValue, use DataValueField.
            ddl.DataValueField = "DisplayValue";
            ddl.DataBind();
            // Optionally, insert a default item.
            ddl.Items.Insert(0, new ListItem("-- Select --", ""));
        }


        /// <summary>
        /// Loads dynamic field definitions from ACC_AssetInfo.
        /// Expected columns: textboxOrder, ColumnOrder, textboxName, FieldMapped.
        /// </summary>
        private void LoadDynamicFields()
        {
            dynamicFields = new List<DynamicField>();
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT textboxOrder, ColumnOrder, textboxName, FieldMapped, dropdown FROM ACC_AssetInfo WHERE AssetID = ? ORDER BY textboxOrder, ColumnOrder";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                conn.Open();
                OdbcDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dynamicFields.Add(new DynamicField
                    {
                        TextboxOrder = Convert.ToInt32(rdr["textboxOrder"]),
                        ColumnOrder = Convert.ToInt32(rdr["ColumnOrder"]),
                        TextboxName = rdr["textboxName"].ToString(),
                        FieldMapped = rdr["FieldMapped"].ToString(),
                        Dropdown = rdr["dropdown"] != DBNull.Value ? Convert.ToInt32(rdr["dropdown"]) : 0
                    });
                }
                conn.Close();
            }
        }

   /*     private void LoadDynamicFields()
        {
            dynamicFields = new List<DynamicField>();
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT textboxOrder, ColumnOrder, textboxName, FieldMapped FROM ACC_AssetInfo WHERE AssetID = ? ORDER BY textboxOrder, ColumnOrder";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                conn.Open();
                OdbcDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    dynamicFields.Add(new DynamicField
                    {
                        TextboxOrder = Convert.ToInt32(rdr["textboxOrder"]),
                        ColumnOrder = Convert.ToInt32(rdr["ColumnOrder"]),
                        TextboxName = rdr["textboxName"].ToString(),
                        FieldMapped = rdr["FieldMapped"].ToString()
                    });
                }
                conn.Close();
            }
        }*/

        // Called when the user clicks the "[Down for maintenance]" button.
        protected void btnDownMaintenance_Click(object sender, EventArgs e)
        {
            pnlEntry.Visible = false;
            pnlMaintenanceDown.Visible = true;
        }

        // Called when the user clicks the Cancel button on the Maintenance Down Panel.
        protected void btnCancelMaintenanceDown_Click(object sender, EventArgs e)
        {
            // Cancel maintenance mode: hide the down panel and show the normal entry screen.
            pnlMaintenanceDown.Visible = false;
            pnlEntry.Visible = true;
        }

        // Called when the user submits the maintenance down information.
        protected void btnSubmitMaintenanceDown_Click(object sender, EventArgs e)
        {
            string initials = txtInitials.Text.Trim();
            if (initials.Length < 2 || initials.Length > 3)
            {
                // Optionally, display an error message.
                return;
            }
            string commentDown = txtMaintenanceComment.Text.Trim();
            DateTime startTime = DateTime.Now;

            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "INSERT INTO ACC_Maintenance (AssetID, StartTime, UserInitials, CommentDown) VALUES (?, ?, ?, ?); SELECT @@IDENTITY;";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                cmd.Parameters.Add("", OdbcType.DateTime).Value = startTime;
                cmd.Parameters.Add("", OdbcType.VarChar).Value = initials;
                cmd.Parameters.Add("", OdbcType.VarChar).Value = commentDown;
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    currentMaintenanceId = Convert.ToInt32(result);
                }
                conn.Close();
            }
            // Hide down panel, show resume panel.
            lblMaintenanceStart.Text = startTime.ToString("yyyy-MM-dd HH:mm:ss");
            pnlMaintenanceDown.Visible = false;
            pnlMaintenanceResume.Visible = true;
            // Optionally, store the startTime in ViewState to calculate time offline.
        }

        // Called when the user clicks the Resume button.
        protected void btnResume_Click(object sender, EventArgs e)
        {
            string resumeComment = txtResumeComment.Text.Trim();
            DateTime resumeTime = DateTime.Now;
            currentMaintenanceId = Convert.ToInt16(cMID.Text);

            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "UPDATE ACC_Maintenance SET ResumeTime = ?, CommentResume = ? WHERE MaintenanceID = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.DateTime).Value = resumeTime;
                cmd.Parameters.Add("", OdbcType.VarChar).Value = resumeComment;
                cmd.Parameters.Add("", OdbcType.Int).Value = currentMaintenanceId;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            pnlMaintenanceResume.Visible = false;
            pnlEntry.Visible = true;
            BindGrid();
        }

        private void CheckMaintenanceStatus()
        {
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT TOP 1 MaintenanceID, StartTime FROM ACC_Maintenance WHERE AssetID = ? AND ResumeTime IS NULL ORDER BY StartTime DESC";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                conn.Open();
                OdbcDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    currentMaintenanceId = Convert.ToInt32(rdr["MaintenanceID"]);
                    DateTime startTime = Convert.ToDateTime(rdr["StartTime"]);
                    lblMaintenanceStart.Text = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                    // Optionally calculate time offline:
                    lblTimeOffline.Text = ((int)(DateTime.Now - startTime).TotalMinutes).ToString();
                    pnlMaintenanceResume.Visible = true;
                    pnlEntry.Visible = false;
                    cMID.Text = currentMaintenanceId.ToString();
                }
                conn.Close();
            }
        }


        /// <summary>
        /// Creates dynamic entry controls in a table layout with up to 4 textboxes per row.
        /// </summary>

        private void CreateDynamicControls()
        {
            phDynamicFields.Controls.Clear();
            // Group dynamic fields by TextboxOrder and order them by ColumnOrder.
            var groups = dynamicFields.GroupBy(f => f.TextboxOrder).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                // Start a new table row.
                phDynamicFields.Controls.Add(new LiteralControl("<tr>"));
                foreach (var field in group.OrderBy(f => f.ColumnOrder))
                {
                    // Start a cell.
                    phDynamicFields.Controls.Add(new LiteralControl("<td style='padding:5px; vertical-align:top;'>"));

                    // Create a label.
                    Label lbl = new Label();
                    lbl.Text = field.TextboxName + ": ";
                    phDynamicFields.Controls.Add(lbl);

                    if (field.Dropdown > 0)
                    {
                        DropDownList ddl = new DropDownList();
                        ddl.ID = "ddl_" + field.FieldMapped;
                        // Only populate if this is not a postback.
                        if (!Page.IsPostBack)
                        {
                            PopulateDropDown(ddl, field.Dropdown);
                        }
                        else
                        {
                            // If Items are not populated, populate them.
                            if (ddl.Items.Count == 0)
                                PopulateDropDown(ddl, field.Dropdown);
                        }
                        field.DropDownListControl = ddl;
                        phDynamicFields.Controls.Add(ddl);
                    }
                    else
                    {
                        TextBox txt = new TextBox();
                        txt.ID = "txt_" + field.FieldMapped;
                        // (Your existing formatting code for textboxes)
                        string fieldLower = field.FieldMapped.ToLower();
                        if (fieldLower.StartsWith("time"))
                        {
                            txt.MaxLength = 5;
                            txt.Attributes.Add("placeholder", "hh:mm");
                            txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                        }
                        else if (fieldLower.StartsWith("int"))
                        {
                            txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                        }
                        else if (fieldLower.StartsWith("float") || fieldLower.StartsWith("weight") || fieldLower.StartsWith("temp"))
                        {
                            txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                        }
                        field.TextBoxControl = txt;
                        phDynamicFields.Controls.Add(txt);
                    }

                    // End the cell.
                    phDynamicFields.Controls.Add(new LiteralControl("</td>"));
                }
                // If fewer than 4 fields in this group, add empty cells.
                int missing = 4 - group.Count();
                for (int i = 0; i < missing; i++)
                {
                    phDynamicFields.Controls.Add(new LiteralControl("<td></td>"));
                }
                // End the row.
                phDynamicFields.Controls.Add(new LiteralControl("</tr>"));
            }
        }

        /*private void CreateDynamicControls()
        {

            phDynamicFields.Controls.Clear();

            // Group dynamic fields by TextboxOrder and order them by ColumnOrder.
            var groups = dynamicFields.GroupBy(f => f.TextboxOrder).OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                // Start a new table row.
                phDynamicFields.Controls.Add(new LiteralControl("<tr>"));
                foreach (var field in group.OrderBy(f => f.ColumnOrder))
                {
                    // Start a cell.
                    phDynamicFields.Controls.Add(new LiteralControl("<td style='padding:5px; vertical-align:top;'>"));

                    // Create a label for the field.
                    Label lbl = new Label();
                    lbl.Text = field.TextboxName + ": ";
                    phDynamicFields.Controls.Add(lbl);

                    // Create the TextBox.
                    TextBox txt = new TextBox();
                    txt.ID = "txt_" + field.FieldMapped;

                    // Apply formatting based on FieldMapped.
                    string fieldLower = field.FieldMapped.ToLower();
                    if (fieldLower.StartsWith("time"))
                    {
                        // For time fields: allow only digits and colon; display hours and minutes.
                        txt.MaxLength = 5; // e.g. "hh:mm"
                        txt.Attributes.Add("placeholder", "hh:mm");
                        txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (fieldLower.StartsWith("int"))
                    {
                        // For integer fields: allow only digits.
                        txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (fieldLower.StartsWith("float") || fieldLower.StartsWith("weight") || fieldLower.StartsWith("temp"))
                    {
                        // For float fields: allow digits and period.
                        txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                    }

                    // Save the control reference for later use.
                    field.TextBoxControl = txt;
                    phDynamicFields.Controls.Add(txt);

                    // End the cell.
                    phDynamicFields.Controls.Add(new LiteralControl("</td>"));
                }
                // If fewer than 4 fields in this group, add empty cells.
                int missing = 4 - group.Count();
                for (int i = 0; i < missing; i++)
                {
                    phDynamicFields.Controls.Add(new LiteralControl("<td></td>"));
                }
                // End the row.
                phDynamicFields.Controls.Add(new LiteralControl("</tr>"));
            }
        }
*/
        public class DynamicGridTemplate : ITemplate
        {
            private string _fieldMapped;
            private bool _isEditTemplate;

            public DynamicGridTemplate(string fieldMapped, bool isEditTemplate)
            {
                _fieldMapped = fieldMapped;
                _isEditTemplate = isEditTemplate;
            }

            public void InstantiateIn(Control container)
            {
                if (_isEditTemplate)
                {
                    TextBox txt = new TextBox();
                    txt.ID = "txt_" + _fieldMapped;
                    // Apply formatting restrictions based on FieldMapped:
                    string lower = _fieldMapped.ToLower();
                    if (lower.StartsWith("time"))
                    {
                        txt.MaxLength = 5; // Format: hh:mm
                        txt.Attributes.Add("placeholder", "hh:mm");
                        txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (lower.StartsWith("int"))
                    {
                        txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (lower.StartsWith("float") || lower.StartsWith("weight") || lower.StartsWith("temp"))
                    {
                        txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                    }
                    txt.DataBinding += new EventHandler(txt_DataBinding);
                    container.Controls.Add(txt);
                }
                else
                {
                    Label lbl = new Label();
                    lbl.DataBinding += new EventHandler(lbl_DataBinding);
                    container.Controls.Add(lbl);
                }
            }

            private void txt_DataBinding(object sender, EventArgs e)
            {
                TextBox txt = (TextBox)sender;
                GridViewRow row = (GridViewRow)txt.NamingContainer;
                object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
                txt.Text = dataValue != null ? dataValue.ToString() : "";
            }

            private void lbl_DataBinding(object sender, EventArgs e)
            {
                Label lbl = (Label)sender;
                GridViewRow row = (GridViewRow)lbl.NamingContainer;
                object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
                lbl.Text = dataValue != null ? dataValue.ToString() : "";
            }
        }

        /// <summary>
        /// Builds the GridView columns (static and dynamic).  
        /// Dynamic columns are added in order (by textboxOrder then ColumnOrder).  
        /// AssetDataID is not added as a column (it is stored in DataKeys).
        /// </summary>
        private void BuildGridColumns()
        {
            gvEntries.Columns.Clear();

            // Static columns.
            BoundField bfDate = new BoundField
            {
                DataField = "EntryDate",
                HeaderText = "Date",
                DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}",
                ReadOnly = true
            };
            gvEntries.Columns.Add(bfDate);

            BoundField bfShift = new BoundField
            {
                DataField = "Shift",
                HeaderText = "Shift",
                ReadOnly = true
            };
            gvEntries.Columns.Add(bfShift);

            // Dynamic columns: use a TemplateField with DynamicFieldTemplate.
              var orderedFields = dynamicFields.OrderBy(f => f.TextboxOrder).ThenBy(f => f.ColumnOrder);
            foreach (var field in orderedFields)
            {
                TemplateField tf = new TemplateField();
                tf.HeaderText = field.TextboxName;
                tf.ItemTemplate = new DynamicFieldTemplate(field.FieldMapped, false, field.Dropdown);
                tf.EditItemTemplate = new DynamicFieldTemplate(field.FieldMapped, true, field.Dropdown);
                gvEntries.Columns.Add(tf);
            }

            /*  foreach (var field in orderedFields)
              {
                  TemplateField tf = new TemplateField();
                  tf.HeaderText = field.TextboxName;
                  // If Dropdown > 0, pass that value; otherwise, pass 0.
                  tf.ItemTemplate = new DynamicFieldTemplate(field.FieldMapped, false, field.Dropdown);
                  tf.EditItemTemplate = new DynamicFieldTemplate(field.FieldMapped, true, field.Dropdown);
                  gvEntries.Columns.Add(tf);
              }*/


            /*         var orderedFields = dynamicFields.OrderBy(f => f.TextboxOrder)
                                                      .ThenBy(f => f.ColumnOrder);
                     foreach (var field in orderedFields)
                     {
                         TemplateField tf = new TemplateField();
                         tf.HeaderText = field.TextboxName;
                         // Use the custom template for display and edit modes.
                         tf.ItemTemplate = new DynamicFieldTemplate(field.FieldMapped, false);
                         tf.EditItemTemplate = new DynamicFieldTemplate(field.FieldMapped, true);
                         gvEntries.Columns.Add(tf);
                     }*/

            // Actions column (for Edit/Delete).
            TemplateField actionsField = new TemplateField();
            actionsField.HeaderText = "Actions";
            actionsField.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, "Item");
            actionsField.EditItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, "Edit");
            gvEntries.Columns.Add(actionsField);

            // DataKeyNames: store both AssetDataID and EntryDate.
            gvEntries.DataKeyNames = new string[] { "AssetDataID", "EntryDate" };
        }

        /// <summary>
        /// Binds the GridView with data from ACC_AssetData.
        /// IMPORTANT: This method only sets DataSource and calls DataBind() (columns are not rebuilt).
        /// </summary>
        private void BindGrid()
        {
            /**/   var orderedFields = dynamicFields.OrderBy(f => f.TextboxOrder).ThenBy(f => f.ColumnOrder);
            List<string> dynCols = orderedFields.Select(f => f.FieldMapped).ToList();
            string dynamicColumns = dynCols.Count > 0 ? ", " + string.Join(", ", dynCols) : "";

            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            DataTable dt = new DataTable();
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT TOP 10 AssetDataID, EntryDate, Shift" + dynamicColumns +
                                " FROM ACC_AssetData WHERE AssetID = ? ORDER BY EntryDate DESC";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(dt);
            }



            //////////////////
            ///




            gvEntries.Columns.Clear();

            // Static columns.
            BoundField bfDate = new BoundField();
            bfDate.DataField = "EntryDate";
            bfDate.HeaderText = "Date";
            bfDate.DataFormatString = "{0:yyyy-MM-dd}";
            bfDate.ReadOnly = true;
            gvEntries.Columns.Add(bfDate);

            BoundField bfShift = new BoundField();
            bfShift.DataField = "Shift";
            bfShift.HeaderText = "Shift";
            bfShift.ReadOnly = true;
            gvEntries.Columns.Add(bfShift);


            foreach (var field in orderedFields)
            {
                TemplateField tf = new TemplateField();
                tf.HeaderText = field.TextboxName;
                tf.ItemTemplate = new DynamicFieldTemplate(field.FieldMapped, false, field.Dropdown);
                tf.EditItemTemplate = new DynamicFieldTemplate(field.FieldMapped, true, field.Dropdown);
                gvEntries.Columns.Add(tf);
            }


            // Actions column (for Edit/Delete).
            TemplateField actionsField = new TemplateField();
            actionsField.HeaderText = "Actions";
            actionsField.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, "Item");
            actionsField.EditItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, "Edit");
            gvEntries.Columns.Add(actionsField);




            // DataKeyNames: store both AssetDataID and EntryDate.
            gvEntries.DataKeyNames = new string[] { "AssetDataID", "EntryDate" };



 

            gvEntries.DataSource = dt;
            gvEntries.DataBind();

        }

        /// <summary>
        /// Returns the current shift based on ACC_Shift.
        /// </summary>
        private string GetCurrentShift(DateTime currentTime)
        {
            string shift = "";
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT ShiftKey, ShiftStart, ShiftEnd FROM ACC_Shift";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                conn.Open();
                OdbcDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    TimeSpan start = TimeSpan.Parse(rdr["ShiftStart"].ToString());
                    TimeSpan end = TimeSpan.Parse(rdr["ShiftEnd"].ToString());
                    TimeSpan now = currentTime.TimeOfDay;
                    if (start <= end)
                    {
                        if (now >= start && now <= end)
                        {
                            shift = rdr["ShiftKey"].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (now >= start || now <= end)
                        {
                            shift = rdr["ShiftKey"].ToString();
                            break;
                        }
                    }
                }
                conn.Close();
            }
            return shift;
        }

        // ---------------- GridView Event Handlers ----------------

        // Add Entry: remains unchanged.

        protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string shift = GetCurrentShift(now);

            List<string> columns = new List<string>();
            List<string> paramPlaceholders = new List<string>();
            List<object> paramValues = new List<object>();
            foreach (var field in dynamicFields)
            {
                columns.Add(field.FieldMapped);
                paramPlaceholders.Add("?");
                if (field.Dropdown > 0)
                {
                    // For dropdown fields, get the selected value as an integer.
                    int selectedInt = 0;
                    int.TryParse(field.DropDownListControl.SelectedValue, out selectedInt);
                    paramValues.Add(selectedInt);
                }
                else
                {
                    paramValues.Add(field.TextBoxControl.Text);
                }
            }

            /* foreach (var field in dynamicFields)
             {
                 columns.Add(field.FieldMapped);
                 paramPlaceholders.Add("?");

                 // If the field is a dropdown, get its selected value; otherwise, get the textbox value.
                 if (field.Dropdown > 0)
                 {
                     // For dropdowns, use the DropDownListControl.
                     paramValues.Add(field.DropDownListControl.SelectedValue);
                 }
                 else
                 {
                     // Otherwise, use the TextBoxControl.
                     paramValues.Add(field.TextBoxControl.Text);
                 }
             }*/

            /*       foreach (var field in dynamicFields)
                   {
                       columns.Add(field.FieldMapped);
                       paramPlaceholders.Add("?");
                       paramValues.Add(field.TextBoxControl.Text);
                   }*/
            string dynamicColumns = columns.Count > 0 ? ", " + string.Join(", ", columns) : "";
            string dynamicPlaceholders = columns.Count > 0 ? ", " + string.Join(", ", paramPlaceholders) : "";

            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "INSERT INTO ACC_AssetData (AssetID, EntryDate, Shift" + dynamicColumns +
                               ") VALUES (?, ?, ?" + dynamicPlaceholders + ")";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                cmd.Parameters.Add("", OdbcType.DateTime).Value = now;
                cmd.Parameters.Add("", OdbcType.VarChar).Value = shift;
                foreach (var val in paramValues)
                {
                    cmd.Parameters.Add("", OdbcType.VarChar).Value = val;
                }
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            BindGrid();
        }

        /*protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string shift = GetCurrentShift(now);

            List<string> columns = new List<string>();
            List<string> paramPlaceholders = new List<string>();
            List<object> paramValues = new List<object>();
            foreach (var field in dynamicFields)
            {
                columns.Add(field.FieldMapped);
                paramPlaceholders.Add("?");
                paramValues.Add(field.TextBoxControl.Text);
            }
            string dynamicColumns = columns.Count > 0 ? ", " + string.Join(", ", columns) : "";
            string dynamicPlaceholders = columns.Count > 0 ? ", " + string.Join(", ", paramPlaceholders) : "";

            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "INSERT INTO ACC_AssetData (AssetID, EntryDate, Shift" + dynamicColumns +
                               ") VALUES (?, ?, ?" + dynamicPlaceholders + ")";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetId;
                cmd.Parameters.Add("", OdbcType.DateTime).Value = now;
                cmd.Parameters.Add("", OdbcType.VarChar).Value = shift;
                foreach (var val in paramValues)
                {
                    cmd.Parameters.Add("", OdbcType.VarChar).Value = val;
                }
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            BindGrid();
        }
        */
        // RowEditing: retrieve EntryDate from DataKeys and allow edit only if it’s today.
        protected void gvEntries_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEntries.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvEntries_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvEntries.EditIndex = -1;
            BindGrid();
        }

        // RowUpdating: uses e.NewValues from BoundFields.
        protected void gvEntries_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int assetDataId = Convert.ToInt32(gvEntries.DataKeys[e.RowIndex].Value);
            // Create a dictionary to hold updated values.
            Dictionary<string, object> updatedValues = new Dictionary<string, object>();

            // For each dynamic field, manually find the TextBox in the edited row.
            foreach (var field in dynamicFields.OrderBy(f => f.TextboxOrder).ThenBy(f => f.ColumnOrder))
            {
                TextBox txt = (TextBox)gvEntries.Rows[e.RowIndex].FindControl("txt_" + field.FieldMapped);
                updatedValues[field.FieldMapped] = txt != null ? txt.Text : "";
            }

            // Build the update command.
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                // Create a SET clause, e.g., "Field1 = ?, Field2 = ?"
                string setClause = string.Join(", ", dynamicFields
                    .OrderBy(f => f.TextboxOrder)
                    .ThenBy(f => f.ColumnOrder)
                    .Select(df => df.FieldMapped + " = ?"));

                string query = "UPDATE ACC_AssetData SET " + setClause + " WHERE AssetDataID = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);

                // Add parameters for each dynamic field value.
                foreach (var field in dynamicFields.OrderBy(f => f.TextboxOrder).ThenBy(f => f.ColumnOrder))
                {
                    cmd.Parameters.Add("", OdbcType.VarChar).Value = updatedValues[field.FieldMapped];
                }
                // Finally, add the AssetDataID parameter.
                cmd.Parameters.Add("", OdbcType.Int).Value = assetDataId;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            gvEntries.EditIndex = -1;
            BindGrid();
        }


        protected void gvEntries_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int assetDataId = Convert.ToInt32(gvEntries.DataKeys[e.RowIndex].Value);
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "DELETE FROM ACC_AssetData WHERE AssetDataID = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = assetDataId;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            BindGrid();
        }

        protected void gvEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Hide Edit/Delete buttons for rows not from today.
            if (e.Row.RowType == DataControlRowType.DataRow && !e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                DateTime entryDate = Convert.ToDateTime(drv["EntryDate"]);
                if (entryDate.Date != DateTime.Today)
                {
                    LinkButton lnkEdit = e.Row.FindControl("lnkEdit") as LinkButton;
                    LinkButton lnkDelete = e.Row.FindControl("lnkDelete") as LinkButton;
                    if (lnkEdit != null)
                        lnkEdit.Visible = false;
                    if (lnkDelete != null)
                        lnkDelete.Visible = false;
                }
            }
        }
    }

    // Dynamic field definition class.
    public class DynamicField
    {
        public int TextboxOrder { get; set; }
        public int ColumnOrder { get; set; }
        public string TextboxName { get; set; }
        public string FieldMapped { get; set; }
        public int Dropdown { get; set; }  // New column from ACC_AssetInfo
        public TextBox TextBoxControl { get; set; }
        public DropDownList DropDownListControl { get; set; }  // For dropdown fields
    }


    public class DynamicGridTemplate : ITemplate
    {
        private string _fieldMapped;
        private bool _isEditTemplate;

        public DynamicGridTemplate(string fieldMapped, bool isEditTemplate)
        {
            _fieldMapped = fieldMapped;
            _isEditTemplate = isEditTemplate;
        }

        public void InstantiateIn(Control container)
        {
            if (_isEditTemplate)
            {
                TextBox txt = new TextBox();
                txt.ID = "txt_" + _fieldMapped;
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    txt.MaxLength = 5; // e.g. "hh:mm"
                    txt.Attributes.Add("placeholder", "hh:mm");
                    txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                }
                else if (lower.StartsWith("int"))
                {
                    txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                }
                else if (lower.StartsWith("float") || lower.StartsWith("weight") || lower.StartsWith("temp"))
                {
                    txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                }
                txt.DataBinding += new EventHandler(this.txt_DataBinding);
                container.Controls.Add(txt);
            }
            else
            {
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler(this.lbl_DataBinding);
                container.Controls.Add(lbl);
            }
        }

        private void txt_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    DateTime dt;
                    if (DateTime.TryParse(dataValue.ToString(), out dt))
                    {
                        txt.Text = dt.ToString("HH:mm");
                    }
                    else if (dataValue.ToString().Length >= 5)
                    {
                        txt.Text = dataValue.ToString().Substring(0, 5);
                    }
                    else
                    {
                        txt.Text = dataValue.ToString();
                    }
                }
                else
                {
                    txt.Text = dataValue.ToString();
                }
            }
            else
            {
                txt.Text = "";
            }
        }

        private void lbl_DataBinding(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            GridViewRow row = (GridViewRow)lbl.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    DateTime dt;
                    if (DateTime.TryParse(dataValue.ToString(), out dt))
                    {
                        lbl.Text = dt.ToString("HH:mm");
                    }
                    else if (dataValue.ToString().Length >= 5)
                    {
                        lbl.Text = dataValue.ToString().Substring(0, 5);
                    }
                    else
                    {
                        lbl.Text = dataValue.ToString();
                    }
                }
                else
                {
                    lbl.Text = dataValue.ToString();
                }
            }
            else
            {
                lbl.Text = "";
            }
        }
    }
    public class DynamicFieldTemplate : ITemplate
    {
        private string _fieldMapped;
        private bool _isEditTemplate;
        private int _dropdownId; // > 0 indicates this field is a dropdown

        public DynamicFieldTemplate(string fieldMapped, bool isEditTemplate, int dropdownId)
        {
            _fieldMapped = fieldMapped;
            _isEditTemplate = isEditTemplate;
            _dropdownId = dropdownId;
        }

        public void InstantiateIn(Control container)
        {
            if (_isEditTemplate)
            {
                if (_dropdownId > 0)
                {
                    // Create a DropDownList for dropdown fields.
                    DropDownList ddl = new DropDownList();
                    ddl.ID = "ddl_" + _fieldMapped;
                    ddl.DataBinding += new EventHandler(this.ddl_DataBinding);
                    // Populate the dropdown from ACC_DropDown.
                    PopulateDropDown(ddl, _dropdownId);
                    container.Controls.Add(ddl);
                }
                else
                {
                    // Create a TextBox for non-dropdown fields.
                    TextBox txt = new TextBox();
                    txt.ID = "txt_" + _fieldMapped;
                    string lower = _fieldMapped.ToLower();
                    if (lower.StartsWith("time"))
                    {
                        txt.MaxLength = 5;
                        txt.Attributes.Add("placeholder", "hh:mm");
                        txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (lower.StartsWith("int"))
                    {
                        txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                    }
                    else if (lower.StartsWith("float") || lower.StartsWith("weight") || lower.StartsWith("temp"))
                    {
                        txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                    }
                    txt.DataBinding += new EventHandler(this.txt_DataBinding);
                    container.Controls.Add(txt);
                }
            }
            else
            {
                // Display mode: use a Label.
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler(this.lbl_DataBinding);
                container.Controls.Add(lbl);
            }
        }

        private void ddl_DataBinding(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            if (ddl == null)
                return;

            GridViewRow row = ddl.NamingContainer as GridViewRow;
            if (row == null || row.DataItem == null)
                return;

            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                int storedValue = 0;
                int.TryParse(dataValue.ToString(), out storedValue);
                ListItem item = ddl.Items.FindByValue(storedValue.ToString());
                if (item != null)
                    ddl.SelectedValue = storedValue.ToString();
                else if (ddl.Items.Count > 0)
                    ddl.SelectedIndex = 0;
            }
        }


        private void txt_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    DateTime dt;
                    if (DateTime.TryParse(dataValue.ToString(), out dt))
                        txt.Text = dt.ToString("HH:mm");
                    else if (dataValue.ToString().Length >= 5)
                        txt.Text = dataValue.ToString().Substring(0, 5);
                    else
                        txt.Text = dataValue.ToString();
                }
                else
                {
                    txt.Text = dataValue.ToString();
                }
            }
            else
            {
                txt.Text = "";
            }
        }

        private void lbl_DataBinding(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            GridViewRow row = (GridViewRow)lbl.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (_dropdownId > 0)
            {
                int storedValue = 0;
                int.TryParse(dataValue?.ToString() ?? "0", out storedValue);
                lbl.Text = GetDisplayValue(_dropdownId, storedValue);
            }
            else
            {
                if (dataValue != null)
                {
                    string lower = _fieldMapped.ToLower();
                    if (lower.StartsWith("time"))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(dataValue.ToString(), out dt))
                            lbl.Text = dt.ToString("HH:mm");
                        else if (dataValue.ToString().Length >= 5)
                            lbl.Text = dataValue.ToString().Substring(0, 5);
                        else
                            lbl.Text = dataValue.ToString();
                    }
                    else
                    {
                        lbl.Text = dataValue.ToString();
                    }
                }
                else
                {
                    lbl.Text = "";
                }
            }
        }

        // Helper method to populate a DropDownList from ACC_DropDown.
        private void PopulateDropDown(DropDownList ddl, int dropDownID)
        {
            if (ddl.Items.Count > 0)
                return;
            string cs = ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            DataTable dt = new DataTable();
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                // Adjust column names as needed. This query assumes ACC_DropDown has OrderId and DisplayValue.
                string query = "SELECT OrderId, DisplayValue FROM ACC_DropDown WHERE DropDownID = ? ORDER BY OrderId";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = dropDownID;
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                da.Fill(dt);
            }
            ddl.DataSource = dt;
            ddl.DataTextField = "DisplayValue";
            ddl.DataValueField = "OrderId";  // We want an integer value
            ddl.DataBind();
            // Insert a default item with value 0.
            ddl.Items.Insert(0, new ListItem("-- Select --", "0"));
        }


        // Helper method to lookup the display value for a dropdown given its dropDownID and stored OrderId.
        private string GetDisplayValue(int dropDownID, int orderId)
        {
            string cs = System.Configuration.ConfigurationManager.ConnectionStrings["MyOdbcConnection"].ConnectionString;
            string displayValue = "";
            using (OdbcConnection conn = new OdbcConnection(cs))
            {
                string query = "SELECT DisplayValue FROM ACC_DropDown WHERE DropDownID = ? AND OrderId = ?";
                OdbcCommand cmd = new OdbcCommand(query, conn);
                cmd.Parameters.Add("", OdbcType.Int).Value = dropDownID;
                cmd.Parameters.Add("", OdbcType.Int).Value = orderId;
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    displayValue = result.ToString();
                }
                conn.Close();
            }
            return displayValue;
        }
    }

    /*
    public class DynamicFieldTemplate : ITemplate
    {
        private string _fieldMapped;
        private bool _isEditTemplate;

        public DynamicFieldTemplate(string fieldMapped, bool isEditTemplate)
        {
            _fieldMapped = fieldMapped;
            _isEditTemplate = isEditTemplate;
        }

        public void InstantiateIn(Control container)
        {
            if (_isEditTemplate)
            {
                TextBox txt = new TextBox();
                txt.ID = "txt_" + _fieldMapped;
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    // For time fields: only allow digits and colon, and display HH:mm.
                    txt.MaxLength = 5;
                    txt.Attributes.Add("placeholder", "hh:mm");
                    txt.Attributes.Add("onkeypress", "return /[0-9:]/.test(String.fromCharCode(event.keyCode));");
                }
                else if (lower.StartsWith("int"))
                {
                    // For integer fields: only allow digits.
                    txt.Attributes.Add("onkeypress", "return /[0-9]/.test(String.fromCharCode(event.keyCode));");
                }
                else if (lower.StartsWith("float") || lower.StartsWith("weight") || lower.StartsWith("temp"))
                {
                    // For float fields: allow digits and a period.
                    txt.Attributes.Add("onkeypress", "return /[0-9.]/.test(String.fromCharCode(event.keyCode));");
                }
                txt.DataBinding += new EventHandler(this.txt_DataBinding);
                container.Controls.Add(txt);
            }
            else
            {
                Label lbl = new Label();
                lbl.DataBinding += new EventHandler(this.lbl_DataBinding);
                container.Controls.Add(lbl);
            }
        }

        private void txt_DataBinding(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            GridViewRow row = (GridViewRow)txt.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    // Format time fields to show only hours and minutes.
                    DateTime dt;
                    if (DateTime.TryParse(dataValue.ToString(), out dt))
                    {
                        txt.Text = dt.ToString("HH:mm");
                    }
                    else if (dataValue.ToString().Length >= 5)
                    {
                        txt.Text = dataValue.ToString().Substring(0, 5);
                    }
                    else
                    {
                        txt.Text = dataValue.ToString();
                    }
                }
                else
                {
                    txt.Text = dataValue.ToString();
                }
            }
            else
            {
                txt.Text = "";
            }
        }

        private void lbl_DataBinding(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            GridViewRow row = (GridViewRow)lbl.NamingContainer;
            object dataValue = DataBinder.Eval(row.DataItem, _fieldMapped);
            if (dataValue != null)
            {
                string lower = _fieldMapped.ToLower();
                if (lower.StartsWith("time"))
                {
                    DateTime dt;
                    if (DateTime.TryParse(dataValue.ToString(), out dt))
                    {
                        lbl.Text = dt.ToString("HH:mm");
                    }
                    else if (dataValue.ToString().Length >= 5)
                    {
                        lbl.Text = dataValue.ToString().Substring(0, 5);
                    }
                    else
                    {
                        lbl.Text = dataValue.ToString();
                    }
                }
                else
                {
                    lbl.Text = dataValue.ToString();
                }
            }
            else
            {
                lbl.Text = "";
            }
        }
    }*/

    // Simple class to hold the limits for a textbox.
    public class Limit
    {
        public string TextboxType { get; set; }
        public double LowerLimitF { get; set; }
        public double UpperLimitF { get; set; }
        public DateTime LowerLimitT { get; set; }
        public DateTime UpperLimitT { get; set; }
    }

    /// <summary>
    /// Implements ITemplate to create a TemplateField for GridView action buttons.
    /// </summary>
    public class GridViewTemplate : ITemplate
    {
        DataControlRowType templateType;
        string columnType; // "Item" for display mode, "Edit" for edit mode

        public GridViewTemplate(DataControlRowType type, string colType)
        {
            templateType = type;
            columnType = colType;
        }

        public void InstantiateIn(Control container)
        {

            if (templateType == DataControlRowType.DataRow)
            {
                if (columnType == "Item")
                {
                    LinkButton lnkEdit = new LinkButton();
                    lnkEdit.ID = "lnkEdit";
                    lnkEdit.CommandName = "Edit";
                    lnkEdit.Text = "Edit";
                    container.Controls.Add(lnkEdit);

                    LiteralControl spacer = new LiteralControl(" ");
                    container.Controls.Add(spacer);

                    LinkButton lnkDelete = new LinkButton();
                    lnkDelete.ID = "lnkDelete";
                    lnkDelete.CommandName = "Delete";
                    lnkDelete.Text = "Delete";
                    lnkDelete.OnClientClick = "return confirm('Are you sure you want to delete this item?');";
                    container.Controls.Add(lnkDelete);
                }
                else if (columnType == "Edit")
                {
                    LinkButton lnkUpdate = new LinkButton();
                    lnkUpdate.ID = "lnkUpdate";
                    lnkUpdate.CommandName = "Update";
                    lnkUpdate.Text = "Update";
                    container.Controls.Add(lnkUpdate);

                    LiteralControl spacer = new LiteralControl(" ");
                    container.Controls.Add(spacer);

                    LinkButton lnkCancel = new LinkButton();
                    lnkCancel.ID = "lnkCancel";
                    lnkCancel.CommandName = "Cancel";
                    lnkCancel.Text = "Cancel";
                    container.Controls.Add(lnkCancel);
                }
            }
        }
    }
}

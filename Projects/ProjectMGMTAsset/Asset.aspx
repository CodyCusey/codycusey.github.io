<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Asset.aspx.cs" Inherits="Production.Asset" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Asset Details</title>
  <style type="text/css">
    /* Header styles */
    .headerContainer {
      width: 100%;
      overflow: hidden;
      margin-bottom: 20px;
    }
    .logo {
      float: right;
      margin-top: 10px;
    }
    /* Entry area table styles */
    .entryTable {
      width: 100%;
      border-collapse: collapse;
      margin-bottom: 10px;
    }
    .entryTable td {
      padding: 5px;
      vertical-align: top;
    }
    .entryTable input[type="text"] {
      width: 95%;
      padding: 4px;
      box-sizing: border-box;
    }
  </style>
  <script type="text/javascript">
      // Limits object (populated from the server) for the numeric textboxes.
      var limits = <%= LimitsJson %>;
      function validateInputs() {
          var valid = true;
          for (var key in limits) {
              var textbox = document.getElementById(key);
              if (textbox) {
                  var value = parseFloat(textbox.value);
                  if (!isNaN(value)) {
                      var type = limits[key].TexboxType;
                      var lower;
                      var upper;
                      if (type = 'Float') {
                          lower = parseFloat(limits[key].LowerLimitF);
                          upper = parseFloat(limits[key].UpperLimitF);
                      }
                      else if (type = 'Int') {
                          lower = parseInt(limits[key].LowerLimitF);
                          upper = parseInt(limits[key].UpperLimitF);
                      }
                      else if (type = 'Time') {
                          lower = limits[key].LowerLimitT;
                          upper = limits[key].UpperLimitT;
                      }
                      if (value < lower) {
                          if (!confirm("The value for " + key + " is lower than expected (" + lower + "). Are you sure?")) {
                              valid = false;
                              break;
                          }
                      } else if (value > upper) {
                          if (!confirm("The value for " + key + " is higher than expected (" + upper + "). Are you sure?")) {
                              valid = false;
                              break;
                          }
                      }
                  }
              }
          }
          return valid;
      }
  </script>
</head>
<body>
  <form id="form1" runat="server">
    <!-- Header with Logo -->
      <div class="header">
    <!-- Logo centered on its own row -->
    <div class="logo" style="width:100%; text-align:center;">
        <a href="/Default.aspx">
            <img src="Images/ACC.png" alt="Logo" style="max-height:80px;" />
        </a>
    </div>
    <!-- Welcome message on its own row, aligned to the right -->
    <div class="welcome" style="width:100%; text-align:right; margin-top:10px; font-size:18px; font-weight:bold;">
        <asp:Label ID="lblWelcome" runat="server" Text="" />
    </div>
</div>

    <div class="headerContainer">
      <h2 style="float:left;">Asset: <asp:Label ID="lblAssetName" runat="server" /></h2>
      <div class="logo">
  
      <div style="clear:both;"></div>
    </div>
    </div>
<!-- Down for Maintenance Button -->
<asp:Button ID="Button1" runat="server" Text="Down for maintenance" OnClick="btnDownMaintenance_Click" />

<!-- Maintenance Down Panel (initially hidden) -->
<asp:Panel ID="pnlMaintenanceDown" runat="server" Visible="false" Style="border:1px solid #ccc; padding:10px; margin-top:10px;">
    <table>
        <tr>
            <td>Enter your initials (2-3 characters):</td>
            <td><asp:TextBox ID="txtInitials" runat="server" MaxLength="3" /></td>
        </tr>
        <tr>
            <td>Comment (optional):</td>
            <td><asp:TextBox ID="txtMaintenanceComment" runat="server" TextMode="MultiLine" Rows="3" Columns="40" /></td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSubmitMaintenanceDown" runat="server" Text="Submit" OnClick="btnSubmitMaintenanceDown_Click" />
            </td>
            <td align="center">
                <asp:Button ID="btnCancelMaintenanceDown" runat="server" Text="Cancel" OnClick="btnCancelMaintenanceDown_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>

<!-- Maintenance Resume Panel (initially hidden) -->
<asp:Panel ID="pnlMaintenanceResume" runat="server" Visible="false" Style="border:1px solid #ccc; padding:10px; margin-top:10px;">
    <table>
        <tr>
            <td>Asset is down since:</td>
            <td><asp:Label ID="lblMaintenanceStart" runat="server" /></td>
            <td><asp:Label ID="cMID" runat="server" Visible="False" /></td>
        </tr>
        <tr>
            <td>Time Offline:</td>
            <td><asp:Label ID="lblTimeOffline" runat="server" /></td>
        </tr>
        <tr>
            <td>Resume Comment (optional):</td>
            <td><asp:TextBox ID="txtResumeComment" runat="server" TextMode="MultiLine" Rows="2" Columns="40" /></td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnResume" runat="server" Text="Resume" OnClick="btnResume_Click" />
            </td>
        </tr>
    </table>
</asp:Panel>


    <!-- Data Entry Panel -->
    <asp:Panel ID="pnlEntry" runat="server">
      <!-- Current Date and Shift -->
      <table>
        <tr>
          <td><asp:Label ID="lblDate" runat="server" Text="Date:" /></td>
          <td><asp:Label ID="lblCurrentDate" runat="server" /></td>
          <td style="padding-left:20px;"><asp:Label ID="lblShift" runat="server" Text="Shift:" /></td>
          <td><asp:Label ID="lblCurrentShift" runat="server" /></td>
        </tr>
      </table>
      <br />
      <!-- Entry Area Table for Dynamic Fields -->
      <table class="entryTable">
        <asp:PlaceHolder ID="phDynamicFields" runat="server" />
      </table>
      <br />
      <asp:Button ID="btnAddEntry" runat="server" Text="Add Entry"
                  OnClick="btnAddEntry_Click" OnClientClick="return validateInputs();" />
    </asp:Panel>
    
    <hr />
    <h3>Latest Entries</h3>
    <!-- DataKeyNames includes both AssetDataID and EntryDate -->
    <asp:GridView ID="gvEntries" runat="server" AutoGenerateColumns="false" EnableViewState="true" 
                  DataKeyNames="AssetDataID"
                  OnRowEditing="gvEntries_RowEditing" OnRowUpdating="gvEntries_RowUpdating"
                  OnRowCancelingEdit="gvEntries_RowCancelingEdit" OnRowDeleting="gvEntries_RowDeleting"
                  OnRowDataBound="gvEntries_RowDataBound">
    </asp:GridView>
  </form>
</body>
</html>

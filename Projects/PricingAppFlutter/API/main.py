from fastapi import FastAPI
from sqlalchemy import create_engine, text
from sqlalchemy.orm import sessionmaker
import json
import ast

config = {
  "sqldbIP": "127.0.0.1",
  "sqldbPort": "60611",
  "sqldbDB": "Unpriced",
  "sqldbUserId": "appuser",
  "sqldbPass": "password"
}

app = FastAPI()

def retrieve_data_from_table(session):
    query = text("SELECT id, po_number, line_number, vendor_name FROM all_unpriced")
    cursor = session.execute(query)

    id = []
    po_number = []
    line_number = []
    vendor_name = []
    json_objects = []

    for row in cursor.fetchall():
        id.append(row[0])
        po_number.append(row[1])
        line_number.append(row[2])
        vendor_name.append(row[3])

    for i in range(len(id)):
        order = {
            "id": id[i],
            "po_number": po_number[i],
            "line_number": line_number[i],
            "vendor_name": vendor_name[i]
        }
        json_objects.append(order)

    # json_dic = {"id": id, "po_number": po_number, "line_number": line_number, "vendor_name": vendor_name} #** Kind of Works **
    # json_dic = {id + po_number + line_number + vendor_name}

    # json_data = {tuple(id): id, tuple(po_number): po_number, tuple(line_number): line_number, tuple(vendor_name): vendor_name}
    # json_data = {json.dumps(id): id, json.dumps(po_number): po_number} ** Kind of Works! **
    json_list = json.dumps(json_objects)
    # json_string = json.loads(json_list)
    json_data = ast.literal_eval(json_list)

    cursor.close()
    session.close()

    return json_data

@app.get("/")
async def home():
    return retrieve_data_from_table(session)

# Start of program, connecting to DB
conn_string = f"mssql+pyodbc://{config['sqldbUserId']}:{config['sqldbPass']}@{config['sqldbIP']}:{config['sqldbPort']}/{config['sqldbDB']}?driver=ODBC+Driver+17+for+SQL+Server"
connection = create_engine(conn_string)

Session = sessionmaker(bind=connection)
session = Session()

# Run the following command in PyCharm Terminal window to start the FastAPI connection via Uvicorn Server (integrated)
# python -m uvicorn main:app --reload

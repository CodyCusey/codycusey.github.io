from sqlalchemy import text
import json

def retrieve_data_from_table(session):
    query = text("SELECT * FROM all_unpriced")
    cursor = session.execute(query)

    po_number = []
    vendor_name = []
    json_data = {}

    for row in cursor.fetchall():
        po_number.append(row[0])
        vendor_name.append(row[1])

    json_data = {po_number: po_number, vendor_name: vendor_name}

    cursor.close()
    session.close()

    return json_data

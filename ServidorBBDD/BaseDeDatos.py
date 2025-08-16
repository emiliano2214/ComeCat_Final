from flask import Flask, jsonify, request
import sqlite3

app = Flask(__name__)

def get_connection():
    return sqlite3.connect("gatos.db")

@app.route('/db/status')
def db_status():
    return jsonify({"db": "Conectado a SQLite"})

@app.route('/db/dispensadores', methods=['GET'])
def get_all():
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM RegistroDispensador")
    rows = cur.fetchall()
    con.close()
    # Convertimos cada registro a JSON
    result = [
        {"Id": r[0], "Proximidad": r[1], "ServoActivo": r[2],
         "FechaDispensacion": r[3], "HoraDispensacion": r[4]}
        for r in rows
    ]
    return jsonify(result)

@app.route('/db/dispensadores/<int:id>', methods=['GET'])
def get_by_id(id):
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM RegistroDispensador WHERE Id=?", (id,))
    row = cur.fetchone()
    con.close()
    if row:
        return jsonify({"Id": row[0], "Proximidad": row[1], "ServoActivo": row[2],
                        "FechaDispensacion": row[3], "HoraDispensacion": row[4]})
    else:
        return jsonify({"error": "No encontrado"}), 404

@app.route('/db/dispensadores', methods=['POST'])
def add():
    data = request.get_json()
    con = get_connection()
    cur = con.cursor()
    cur.execute("INSERT INTO RegistroDispensador (Proximidad, ServoActivo, FechaDispensacion, HoraDispensacion) VALUES (?, ?, ?, ?)",
                (data["Proximidad"], data["ServoActivo"], data["FechaDispensacion"], data["HoraDispensacion"]))
    con.commit()
    new_id = cur.lastrowid
    con.close()
    return jsonify({"Id": new_id}), 201

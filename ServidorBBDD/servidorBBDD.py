from flask import Flask, jsonify, request
import sqlite3

app = Flask(__name__)
DB_FILE = "gatos_resultados.db"  # Una sola DB para todo

def get_connection():
    return sqlite3.connect(DB_FILE)

# ----------------- STATUS -----------------
@app.route('/db/status')
def db_status():
    return jsonify({"db": "Conectado a SQLite - Unificada"})

# ----------------- REGISTRO DISPENSADOR -----------------
@app.route('/db/dispensadores', methods=['GET'])
def get_all_dispensadores():
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM RegistroDispensador")
    rows = cur.fetchall()
    con.close()
    return jsonify([
        {"Id": r[0], "Proximidad": r[1], "ServoActivo": r[2],
         "FechaDispensacion": r[3], "HoraDispensacion": r[4]} for r in rows
    ])

@app.route('/db/dispensadores/<int:id>', methods=['GET'])
def get_dispensador_by_id(id):
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM RegistroDispensador WHERE Id=?", (id,))
    row = cur.fetchone()
    con.close()
    if row:
        return jsonify({"Id": row[0], "Proximidad": row[1], "ServoActivo": row[2],
                        "FechaDispensacion": row[3], "HoraDispensacion": row[4]})
    return jsonify({"error": "No encontrado"}), 404

@app.route('/db/dispensadores', methods=['POST'])
def add_dispensador():
    data = request.get_json()
    con = get_connection()
    cur = con.cursor()
    cur.execute("""INSERT INTO RegistroDispensador 
                   (Proximidad, ServoActivo, FechaDispensacion, HoraDispensacion)
                   VALUES (?, ?, ?, ?)""",
                (data["Proximidad"], data["ServoActivo"],
                 data["FechaDispensacion"], data["HoraDispensacion"]))
    con.commit()
    new_id = cur.lastrowid
    con.close()
    return jsonify({"Id": new_id}), 201

# ----------------- RESULTADOS TEST -----------------
@app.route('/db/resultados', methods=['GET'])
def get_all_resultados():
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM ResultadosTest")
    rows = cur.fetchall()
    con.close()
    return jsonify([{"Id": r[0], "Mensaje": r[1], "Fecha": r[2]} for r in rows])

@app.route('/db/resultados/<int:id>', methods=['GET'])
def get_resultado_by_id(id):
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM ResultadosTest WHERE Id=?", (id,))
    row = cur.fetchone()
    con.close()
    if row:
        return jsonify({"Id": row[0], "Mensaje": row[1], "Fecha": row[2]})
    return jsonify({"error": "No encontrado"}), 404

@app.route('/db/resultados', methods=['POST'])
def add_resultado():
    data = request.get_json()
    if "Mensaje" not in data or not data["Mensaje"]:
        return jsonify({"error": "Mensaje requerido"}), 400
    con = get_connection()
    cur = con.cursor()
    cur.execute("""INSERT INTO ResultadosTest (Mensaje, Fecha)
                   VALUES (?, datetime('now'))""",
                (data["Mensaje"],))
    con.commit()
    new_id = cur.lastrowid
    con.close()
    return jsonify({"Id": new_id}), 201

# ----------------- CREAR TABLAS -----------------
def init_db():
    con = sqlite3.connect(DB_FILE)
    con.execute("""
        CREATE TABLE IF NOT EXISTS RegistroDispensador (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Proximidad REAL NOT NULL,
            ServoActivo INTEGER NOT NULL,
            FechaDispensacion TEXT NOT NULL,
            HoraDispensacion TEXT NOT NULL
        )
    """)
    con.execute("""
        CREATE TABLE IF NOT EXISTS ResultadosTest (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Mensaje TEXT NOT NULL,
            Fecha TEXT NOT NULL
        )
    """)
    con.close()

if __name__ == '__main__':
    init_db()
    app.run(host='0.0.0.0', port=5002)

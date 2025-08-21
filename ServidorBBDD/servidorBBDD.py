from flask import Flask, jsonify, request
import sqlite3

app = Flask(__name__)

def get_connection():
    return sqlite3.connect("gatos.db")

# ---------- STATUS ----------
@app.route('/db/status')
def db_status():
    return jsonify({"db": "Conectado a SQLite"})

# ---------- GET ALL ----------
@app.route('/db/dispensadores', methods=['GET'])
def get_all():
    con = get_connection()
    cur = con.cursor()
    cur.execute("SELECT * FROM RegistroDispensador")
    rows = cur.fetchall()
    con.close()
    result = [
        {"Id": r[0], "Proximidad": r[1], "ServoActivo": r[2],
         "FechaDispensacion": r[3], "HoraDispensacion": r[4]}
        for r in rows
    ]
    return jsonify(result)

# ---------- GET BY ID ----------
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

# ---------- INSERT ----------
@app.route('/db/dispensadores', methods=['POST'])
def add():
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

# ---------- BLOQUE PRINCIPAL ----------
if __name__ == '__main__':
    # Asegurar que la tabla exista
    con = sqlite3.connect("gatos.db")
    con.execute("""
        CREATE TABLE IF NOT EXISTS RegistroDispensador (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Proximidad REAL NOT NULL,
            ServoActivo INTEGER NOT NULL,
            FechaDispensacion TEXT NOT NULL,
            HoraDispensacion TEXT NOT NULL
        )
    """)

    # Insertar un registro inicial si no existe
    cur = con.cursor()
    cur.execute("""
        SELECT COUNT(*) FROM RegistroDispensador 
        WHERE Proximidad=? AND ServoActivo=? AND FechaDispensacion=? AND HoraDispensacion=?
    """, (12.5, 1, "2025-08-17", "19:20:00"))
    
    if cur.fetchone()[0] == 0:
        cur.execute("""
            INSERT INTO RegistroDispensador (Proximidad, ServoActivo, FechaDispensacion, HoraDispensacion)
            VALUES (?, ?, ?, ?)
        """, (12.5, 1, "2025-08-17", "19:20:00"))
        con.commit()
        print("Registro inicial agregado.")

    con.close()

    # Ejecutar servidor Flask en el puerto correcto
    app.run(host='0.0.0.0', port=5002)

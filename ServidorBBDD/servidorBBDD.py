import sqlite3
from flask import Flask, jsonify

app = Flask(__name__)

@app.route('/db/status')
def db_status():
    con = sqlite3.connect("gatos.db")
    return jsonify({"db": "Conectado a SQLite"})

if __name__ == '__main__':
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
    con.close()
    app.run(host='0.0.0.0', port=5001)

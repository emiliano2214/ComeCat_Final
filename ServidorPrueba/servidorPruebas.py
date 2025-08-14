import requests

def probar_api():
    try:
        r = requests.get("http://api:5000/api/status")  # nombre del servicio en docker-compose
        print("Prueba API:", r.json())
    except Exception as e:
        print("No se pudo conectar a la API:", e)

if __name__ == '__main__':
    probar_api()

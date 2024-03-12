from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

BASE_URL = "https://localhost:7240/api/Auth"

@app.route('/api/data', methods=['POST'])
def post_data():
    # Here you can process the data sent from the front end
    data = request.get_json()
    response = requests.post(f"{BASE_URL}/data", json=data)
    return jsonify(response.json()), response.status_code

@app.route('/api/data', methods=['PUT'])
def put_data():
    # Here you can update the data
    data = request.get_json()
    response = requests.put(f"{BASE_URL}/data", json=data)
    return jsonify(response.json()), response.status_code

@app.route('/api/data/<id>', methods=['DELETE'])
def delete_data(id):
    # Here you can delete the data with the given id
    response = requests.delete(f"{BASE_URL}/data/{id}")
    return jsonify(response.json()), response.status_code

@app.route('/api/auth/login', methods=['POST'])
def login():
    # Here you can authenticate the user
    data = request.get_json()
    username = data.get('Username')
    password = data.get('Password')
    # Authenticate the user using the username and password
    response = requests.post(f"{BASE_URL}/login", json={"Username": username, "Password": password})
    return jsonify(response.json()), response.status_code

@app.route('/api/auth/register', methods=['POST'])
def register():
    # Here you can register a new user
    data = request.get_json()
    # Register the user using the data
    response = requests.post(f"{BASE_URL}/register", json=data)
    return jsonify(response.json()), response.status_code

@app.route('/api/images', methods=['GET'])
def get_images():
    response = requests.get(f"{BASE_URL}/Image/DisplayImages")
    return jsonify(response.json()), response.status_code

if __name__ == '__main__':
    app.run(debug=True)
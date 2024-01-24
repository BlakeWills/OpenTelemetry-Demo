import base64
import requests
import time

users = [
    'bob:p@55w0rd',
    'alice:p@55w0rd'
]

auth_headers = []
for user in users:
    b64Bytes = base64.b64encode(user.encode('utf-8'))
    auth_headers.append(b64Bytes.decode('utf-8'))

print('starting main loop')

i = 0
while True:
    try:
        user = auth_headers[i % len(auth_headers)]
        response = requests.get('https://weatherapi/WeatherForecast', verify=False, headers={'Authorization': f'Basic {user}'})
        print(f'user: {user}, status_code: {response.status_code}, content: {response.content}', flush=True)
    except Exception as e:
        print(e, flush=True)
        
    i += 1
    time.sleep(1)

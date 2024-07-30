from http.server import BaseHTTPRequestHandler
import json
import os 
import socket

def get_value_for(content : str, name : str):
    return [i.split("=")[1] for i in content.split("&") if i.startswith(name)][0]

def from_json():
    
    try:
        f = open("save.txt", "x+") if not os.path.exists("save.txt") else open("save.txt", "r")
        content = f.read()
        return json.loads(content)
    except Exception as e:
        print(e)
        return {}

highscores = from_json()

def to_json(content : dict[str, int]):
    f = open("save.txt", "w")
    f.write(str(content).replace("'", '"'))

class PostHandler(BaseHTTPRequestHandler):
    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/html')
        self.end_headers()

    def do_POST(self):
        global highscores
        content_length = int(self.headers['Content-Length']) # <--- Gets the size of data
        post_data = self.rfile.read(content_length) # <--- Gets the data itself
        content = str(post_data).split("'")[1]
        
        player = get_value_for(content, "name")
        score = int(get_value_for(content, "score"))
        
        if(not player or not score): return

        highscores[player] = score
        highscores = dict(sorted(highscores.items(), key=lambda item : item[1]))

        to_json(highscores)
        print("POST request,\nPath: %s\nHeaders:\n%s\n\nBody:\n%s\n",
                str(self.path), str(self.headers), post_data.decode('utf-8'))

        self._set_response()
        self.wfile.write(str(highscores).encode("utf-8"))


from http.server import HTTPServer
host = socket.gethostname()
ip = socket.gethostbyname(host)
server = HTTPServer((ip, 8080), PostHandler)
print('Starting server, use <Ctrl-C> to stop')
server.serve_forever()
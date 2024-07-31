from http.server import BaseHTTPRequestHandler
from util import get_value_for, to_json, from_json, add_score, Entry
import json

class PostHandler(BaseHTTPRequestHandler):
    highscores : list[Entry]

    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'text/plain')
        self.end_headers()

    def do_POST(self):
        self.highscores = from_json()

        content_length = int(self.headers['Content-Length']) # <--- Gets the size of data
        post_data = self.rfile.read(content_length) # <--- Gets the data itself
        content = json.loads(post_data)
        
        player = content["name"]
        score = content["score"]
        
        if(not player or not score): return

        add_score(self.highscores, player, score)

        to_json(self.highscores)
        print("POST request,\nPath: %s\nHeaders:\n%s\n\nBody:\n%s\n",
                str(self.path), str(self.headers), post_data.decode('utf-8'))

        self._set_response()
        content = str([str(e) for e in self.highscores])
        self.wfile.write(content.encode("utf-8"))


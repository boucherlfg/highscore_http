from util import from_json, get_ip
from post_handler import PostHandler


from http.server import HTTPServer
server = HTTPServer((get_ip(), 8080), PostHandler)
print('Starting server, use <Ctrl-C> to stop')
server.serve_forever()
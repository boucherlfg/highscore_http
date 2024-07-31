from util import console_async
from request_handler import RequestHandler
import threading
from http.server import HTTPServer

ip = "localhost"
port = 3000

server = HTTPServer((ip, port), RequestHandler)
x = threading.Thread(target=console_async)
print('Starting server, use <Ctrl-C> to stop')
x.start()
server.serve_forever()


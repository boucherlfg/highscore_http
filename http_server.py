from util import get_ip
from request_handler import RequestHandler


from http.server import HTTPServer
server = HTTPServer(("localhost", 3000), RequestHandler)
print('Starting server, use <Ctrl-C> to stop')
server.serve_forever()
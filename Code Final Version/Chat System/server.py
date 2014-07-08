from twisted.internet import reactor
from twisted.python import log
import sys
from ChatServer.ChatFactory import ChatFactory
import twisted
from autobahn.websocket import WebSocketServerFactory, \
                               WebSocketServerProtocol, \
                               listenWS

if __name__ == '__main__':
	log.startLogging(sys.stdout)

	try:
		factory = ChatFactory("ws://localhost:9000", debug=False, server='localhost')
		listenWS(factory)
		reactor.run()
	except twisted.internet.error.CannotListenError:
		pass
	
	sys.exit("Chat Server closed")
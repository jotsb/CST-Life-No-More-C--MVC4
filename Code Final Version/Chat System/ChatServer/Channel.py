from twisted.internet import reactor
from twisted.python import log
from collections import defaultdict, deque
import sys,json, random, hashlib, datetime
import urllib2
from autobahn.websocket import WebSocketServerFactory, \
                               WebSocketServerProtocol, \
                               listenWS

DEBUG = False

#The maximum size of the saved previous messages
MAX_MESSAGE_STORAGE = 7

#The maximum number to randomly generate to create the channel id hash
MAX_RANDOM_VALUE = 50000

#########################################################################################################################################
#																Channel Class													 		#
# Abstracts a specific channel of communication between multiple users.																	#
# Used to create public channels, as it has no cap on the number of users. Inherited by PrivateChannel.								 	#
#########################################################################################################################################

class Channel():
	#Construct channel object
	def __init__(self,channelName = None, channelID = None):
		if channelName is not None:
			self.channelName = channelName
		#If the channelID has not been passed in, create it
		if not channelID:
			#Generate a hash value of a random number generated
			randomNumber = random.randrange(MAX_RANDOM_VALUE)
			self.channelID = hashlib.md5(str(randomNumber)).hexdigest()
		else:
			self.channelID = channelID
		self.lastUsed = datetime.datetime.now()
		self.previousMessages = deque([],MAX_MESSAGE_STORAGE)
		self.usersConnected = []
		if DEBUG:
			log.msg('Channel.py Created Channel: ' + str(self.channelID))
	
	#Add user to channel
	def addUser(self,newClient):
		self.usersConnected.append(newClient)
	
	def sendArchivedMessages(self,newClient):
		for msg in self.previousMessages:
			newClient.sendMessage(msg)
			if DEBUG:
				log.msg('Channel: '+ str(self.channelID) + ' Sending archived message to user: ' + newClient.username)

	#If user exists in channel, remove.
	def removeUser(self,oldClient):
		if oldClient in self.usersConnected:
			self.usersConnected.remove(oldClient)
	
	#Send a json message to all connected clients
	def chatSend(self,msg):
		self.lastTimeUsed = datetime.datetime.now()
		for client in self.usersConnected:
			client.sendMessage(msg)
			if DEBUG:
				log.msg('Channel: '+ str(self.channelID) + ' Sending message to user: ' + client.username)

		
		self.previousMessages.append(msg)

	def getChannelName(self, currentClientName = None):
		return self.channelName

#########################################################################################################################################
#																Private Channel Class													#
# Represents direct communication between a maximum of two users. Extends Channel class.												 	#
#########################################################################################################################################

class PrivateChannel(Channel):
	#Construct the private channel object. clients parameter is a list of both clients in the channel.
	def __init__(self,clients):
		#Generate a hash based on the user's usernames
		channelID = getHashOfUsernames(clients)
		Channel.__init__(self,channelID=channelID)

		#Create a map of the assigned names based on other users name
		self.nameMap = {}
		self.nameMap[clients[0].username] = clients[1].username
		self.nameMap[clients[1].username] = clients[0].username

		self.usersConnected = clients

		for client in clients:
			client.joinChannel(self)

	
	#Get the name of he channel given the current name in context
	def getChannelName(self,currentClientName):
		return self.nameMap[currentClientName]

	#Send a json message to all connected clients
	def chatSend(self,msg):
		for client in self.usersConnected:
			client.sendMessage(msg)
			if DEBUG:
				log.msg('Channel: '+ str(self.channelID) + ' Sending message to user: ' + client.username)
		self.previousMessages.append(msg)

	def closeChannel(self):
		for client in self.usersConnected:
			client.sendMessage(self.createQuitMsgJson())
			if DEBUG:
				log.msg('Channel.py Channel: ' + self.channelID + ' removing user ' + client.username)
			#Channel.removeUser(self,client)
		self.usersConnected = []

	def createQuitMsgJson(self):
		errorMsg = {}
		errorMsg['type'] = 'quit'
		errorMsg['payload'] = self.channelID
		return json.dumps(errorMsg)

	#Reopen a closed channel
	def reopenChannel(self,clients):
		self.usersConnected = clients
		for client in clients:
			client.joinChannel(self)
			


def getHashOfUsernames(clients):
	hashString = clients[0].username + clients[1].username
	return hashlib.md5(hashString).hexdigest()
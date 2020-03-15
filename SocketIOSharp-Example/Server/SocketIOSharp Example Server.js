var express = require('express')();
var server = require('http').createServer(express);

var io = require('socket.io')(server);
var port = 9001;

io.set('heartbeat interval', 5000);
io.set('heartbeat timeout', 50000);
// Heartbeat timeout MUST be larger than heartbeat interval.
// If not, SocketIOSharp client will automatically close connection after heartbeat timeout mills.

server.listen(port, function () {
	console.log('Listening on ' + port);
	
	io.on('connection', function (socket) {
		console.log('Client connected!');
		
		socket.on('input', function (input) {
			console.log('Client : ' + input);
			
			socket.emit('echo', input);
		});
		
		socket.on('disconnect', function () {
			console.log('Client disconnected!');
		});
	});
});
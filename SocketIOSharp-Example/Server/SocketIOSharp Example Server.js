var express = require('express')();
var server = require('http').createServer(express);

var io = require('socket.io')(server);
var port = 9001;

server.listen(port, function () {
	console.log('Listening on ' + port);
	
	io.on('connection', function (socket) {
		console.log('Client connected!');
		
		socket.on('input', function (input) {
			console.log('Client : ' + input);
			
			socket.emit('echo', input);
		});

		socket.on('input array', function (input1, input2) {
			console.log('Client : ' + input1 + ', ' + input2);

			socket.emit('echo array', input1, input2);
		});
		
		socket.on('disconnect', function () {
			console.log('Client disconnected!');
		});
	});
});
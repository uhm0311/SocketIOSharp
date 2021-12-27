var server = require('http').createServer();

var io = require('socket.io')(server);
var port = 9001;

server.listen(port, function () {
	console.log('Listening on ' + port);
	
	io.on('connection', function (socket) {
		console.log('Client connected!');
		socket.emit('echo', Buffer.from([0, 1, 2, 3, 4, 5]));
		
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
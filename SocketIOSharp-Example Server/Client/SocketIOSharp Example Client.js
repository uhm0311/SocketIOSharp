var socket = require('socket.io-client')('http://localhost:9001/');

socket.on('connection', function () {
    console.log('connected!');
    socket.emit('input', 'asdasdg');
});

socket.on('echo', function (data) {
    console.log('echo : ' + data);
});

socket.on('disconnect', function () {
    console.log('disconnected!');
});

const readline = require('readline').createInterface({
	input: process.stdin,
	output: process.stdout
});

function onMessage(message) {
	if (message != '/exit') {
		socket.emit('input', Buffer.from([0, 1, 2, 3, 4, 5, 6]));

		socket.emit('input', 'Client says, ');
		socket.emit('input', message);

		socket.emit('input', 'Client says again, ');
		socket.emit('input', 'Hello world!');

		readline.question('', onMessage);
	} else {
		process.exit();
	}
}

console.log('input /exit to exit program.');
socket.connect();

readline.question('', onMessage);
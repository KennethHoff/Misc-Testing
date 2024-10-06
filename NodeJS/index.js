import express from "express";

const port = 3333;

const app = express();

app.listen(port, () =>{
				console.log('We are live on ' + port);
});

app.get('/', (req, res) => {
				res.send('hello');
});


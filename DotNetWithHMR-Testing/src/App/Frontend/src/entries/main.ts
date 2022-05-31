import TestPage from "../pages/TestPage.svelte";
import "../assets/css/tailwind/index.css";

const app = new TestPage({
	target: document.getElementById('app-entry')
})

export default app

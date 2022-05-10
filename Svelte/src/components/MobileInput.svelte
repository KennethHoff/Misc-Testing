<label for="{randomId}">Mobile number</label>
<input class="peer {isValidationShowing ? 'invalid:outline-2 invalid:outline-red-500 invalid:outline-dashed' : ''}"
	   id="{randomId}"
	   maxlength="{phoneNumberLength}"
	   minlength="{phoneNumberLength}"
	   on:input={onlyAllowInputMatchingPattern.bind(this, pattern)}
	   pattern="[0-9]{`{${phoneNumberLength}}`}"
	   required
	   type="tel"
/>
<p class="invisible {isValidationShowing ? 'peer-invalid:visible' : ''}">Invalid input</p>

<script lang="ts">
	import {onlyAllowInputMatchingPattern} from "../utilities/InputUtilities.js";
	import {GenerateRandomId} from "../utilities/RandomUtilities";

	let pattern = "[0-9]";
	let phoneNumberLength = 8;

	const randomId = GenerateRandomId("mi-");
	export let isValidationShowing: boolean;
</script>
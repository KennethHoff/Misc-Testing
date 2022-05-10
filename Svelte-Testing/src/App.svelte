<main>
	<form>
		<MobileInput isValidationShowing="{hasTriedToSubmit}"/>

		<button class="border-2 border-blue-400 p-2 rounded-lg hover:bg-gray-200"
				on:click={onSubmit}
				type="button"
		>Send
		</button>
		<button class="border-2 border-blue-400 p-2 rounded-lg hover:bg-gray-200"
				on:click={onUnSubmit}
				type="button"
		>Unsend
		</button>
	</form>
</main>

<script lang="ts">

	import MobileInput from "./components/MobileInput.svelte";

	let hasTriedToSubmit = false;

	function onSubmit(): void {
		hasTriedToSubmit = true;
	}

	function onUnSubmit(): void {
		hasTriedToSubmit = false;
	}

</script>


# ASP.Net Core 6 Solution with Svelte and Vite with working HMR

Etter frontend møte var det snakk om at vi ikke har HMR(`Hot Module Reloading`) på grunn av at vi jobber med pseudo-SSR løsninger ved hjelp av ASP.Net. 
Jeg har også antatt at det er en problemstilling som vi ikke kunne ha gjort noe med, men på vei hjem fra det møtet så tenkte jeg litt på det, og så følte jeg at det burde egentlig være mulig å få til.

Jeg tok meg da litt tid til å sjekke om det faktisk var mulig å få til, og 2.5 timer senere så fikk jeg det til.

Etter en tidlig fungerende prototype, så jobbet jeg ~3.5-4 timer til, og fikk det til å kompilere riktig for Production også, samt la til noen andre biblioteker, bare for å sjekke om det fungerte bra selv med litt mer komplisert struktur


## Hvordan det fungerer

Etter du kjører `npm run build` handlingen i node(Under `src/App/Frontend/`), så vil den putte alle filene i en `dist` mappe. Denne mappen blir lagt under `src/app/wwwroot/dist/` - som da er `staticFile` mappen til ASP.Net Core. Alle filene under der vil være tilgjengelige (Dette vil da si at alle filene som blir produsert, vil være tilgjengelig over nettet). Inne i `dist` mappen så finner du også en fil `manifest.json`. Dette er så-og-si indeksen mellom Entry filene til {JS Rammeverk} og JS/CSS filene de bruker. Når du da skal finne en Page Entry, så leter du bare gjennom den Manifest.json filen, og legger til riktige `<link/>` og `<script/>` tags ettersom.

## Hvis du vil teste det.

- Installer den nyeste versjonen av `NPM`.

   - `nvm install latest; nvm use latest`.

- Last ned repo

  - `git clone`, `download zip` -- up to you.

- Åpne .sln filen til `src/app/`, og kjør `App - Development` 

- Kjør `pnpm/yarn/npm install`(Jeg brukte PNPM, men Yarn fungerer sikkert) i `src/app/Frontend` mappen.

- Kjør `pnpm/yarn/npm dev` i `src/app/Frontend` mappen.

### Hvis du vil teste HMR

Åpne `src/App/Frontend/src/pages/TestPage.svelte` eller en CSS fil i `src/App/Frontend/src/assets/css/tailwind/` og gjør en endring, og du burde se nettleseren oppdatere seg umiddelbart med en gang du lagrer.

# ASP.Net Core 6 Solution with Svelte and Vite with working HMR

Etter frontend møte var det snakk om at vi ikke har HMR(`Hot Module Reloading`) på grunn av at vi jobber med pseudo-SSR løsninger ved hjelp av ASP.Net. 
Jeg har også antatt at det er en problemstilling som vi ikke kunne ha gjort noe med, men på vei hjem fra det møtet så tenkte jeg litt på det, og så følte jeg at det burde egentlig være mulig å få til.

Jeg tok meg da litt tid til å sjekke om det faktisk var mulig å få til, og 2.5 timer senere så fikk jeg det til.

Etter en tidlig fungerende prototype, så jobbet jeg ~3.5-4 timer til, og fikk det til å kompilere riktig for Production også, samt la til noen andre biblioteker, bare for å sjekke om det fungerte bra selv med litt mer komplisert struktur

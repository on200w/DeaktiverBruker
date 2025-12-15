# Deaktiver Bruker

Kort, oppdatert beskrivelse av programmet og hvordan du bruker det.

## Hva programmet gjør
- Deaktiverer den lokale kontoen `Bruker` på valgt tidspunkt.
- Viser en legal notice ved innlogging (standardtekst: "Låne tid utløpt").
- Logger av aktive `Bruker`-økter når deaktiveringen gjennomføres.
- Oppretter en engangsoppgave i Oppgaveplanlegger under `\DeaktiverBruker\`.
- Tilbyr et rydde-verktøy som reaktiverer kontoen, fjerner legal notice og sletter oppgaven.

## Viktige endringer siden forrige versjon
- GUI: WinForms med mørkt tema og tilpassede kontroller.
- Klokkeslett-inngang: bruker en masket input (`HH:MM`) slik at kolon (`:`) ikke kan slettes.
- Publisering: støtter single-file publish til `publish\`-mappen.
- Planlegging: fortsatt implementert via PowerShell ScheduledTasks med `schtasks.exe`-fallback.

## Slik bruker du GUI-en
1. Start `publish\DeaktiverBruker.exe` som administrator (UAC → Ja).
2. Velg dato og klokkeslett (HH:MM). Tidspunktet tastes i en masket boks (f.eks. `10:30`).
3. Klikk `Planlegg deaktivering` for å opprette oppgaven.
4. For å rydde opp: åpne programmet som administrator og velg `Aktiver bruker og rydd opp`.

## Bygg og publiser
1. Bygg (lokal utvikling):

```pwsh
dotnet build -c Release
```

2. Publiser single-file til `publish\` (eksempel for win-x64):

```pwsh
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o publish
```

3. Start den publiserte exe-en som administrator: `publish\DeaktiverBruker.exe`.

## Verifisering (PowerShell)
- Se oppgaver og neste kjøring:

```powershell
Get-ScheduledTask -TaskPath "\DeaktiverBruker\" | Format-Table TaskName, State, NextRunTime -Auto
```

- Se triggere og konto for en oppgave:

```powershell
Get-ScheduledTask -TaskPath "\DeaktiverBruker\" -TaskName <Navn> | Select-Object -ExpandProperty Triggers
Get-ScheduledTask -TaskPath "\DeaktiverBruker\" -TaskName <Navn> | Select-Object TaskName,@{n='User';e={$_.Principal.UserId}}
```

## Forutsetninger
- Windows 10/11 med Oppgaveplanlegger tilgjengelig.
- Programmet må kjøres som administrator for å lage oppgaver, endre konto og skrive i registeret.
- .NET Desktop Runtime installert på målmaskinen.

## Sikkerhet og test
- Test alltid i et kontrollert miljø før du kjører i produksjon.
- Operasjoner som `net user` og `logoff` krever administrative rettigheter og bør brukes forsiktig.

## Support
Hvis noe ikke fungerer som forventet, kontakt IT-ansvarlig og inkluder informasjon om hvordan du startet programmet (f.eks. fra `publish\\DeaktiverBruker.exe`) og eventuelle feilmeldinger.

---
Oppdateringer og endringer i UI eller oppførelsen av kontrollene bør dokumenteres her ved behov.

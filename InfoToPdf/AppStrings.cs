using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoToPdf
{
    [Serializable]
    public class AppStrings
    {
        public string[] StartupTips = new string[] {
                "Be kunden om å velge et passord som er minst 8 tegn langt med 1 stor bokstav og minst 1 tall. Max 16 tegn."
                , "E-post adressen kan også brukes til å logge inn på Elkjøp Cloud."
                , "Skjema kan fylles ut for hånd også. Bare merk av boksene du trenger og skriv ut."
                , "Dokumentet har to forskjellige utseender som kan endres under Innstillinger."
                , "Hvis informasjonen tar mer plass enn èn side, huk av noen av kontoene og skriv de ut seperat etterpå."
                , "Hvis ordrenummer legges til, lages det en strekkode som kan leses inn i Elguide."
                , "Hele navnet på butikken kan fylles ut under innstilligene og vil bli satt inn i bunnteksten."
                };

        public string[] StartupTipsLefdal = new string[] {
                "Be kunden om å velge et passord som er minst 8 tegn langt med 1 stor bokstav og minst 1 tall. Max 16 tegn."
                , "E-post adressen kan også brukes til å logge inn på Lefdal Cloud."
                , "Skjema kan fylles ut for hånd også. Bare merk av boksene du trenger og skriv ut."
                , "Dokumentet har to forskjellige utseender som kan endres under Innstillinger."
                , "Hvis informasjonen tar mer plass enn èn side, huk av noen av kontoene og skriv de ut seperat etterpå."
                , "Hvis ordrenummer legges til, lages det en strekkode som kan leses inn i Elguide."
                , "Hele navnet på butikken kan fylles ut under innstilligene og vil bli satt inn i bunnteksten."
                };

        public string DocGreetCustomerStart = "Takk for at du har benyttet deg av våre teknikere til å få satt opp ditt produkt. Her har du en oversikt over brukernavn og passord knyttet til dine kontoer.<br />";
        public string DocGreetCustomerEnd = "<b>Ta godt vare på denne informasjonen!</b>";
        public string ChainNameElkjop = "Elkjøp";
        public string ChainNameLefdal = "Lefdal";

        public string DocFooterCallcenterElkjop = "Kundeservice: 815 32 000 &nbsp;&nbsp; Åpningstider: Man - fre: 09:00 - 21:00 (lørdag 10:00 - 15:00)";
        public string DocFooterCallcenterLefdal = "Kundeservice: 815 58 800 &nbsp;&nbsp; Åpningstider: Man - fre: 09:00 - 21:00 (lørdag 10:00 - 15:00)";

        public string DocFooterSupportcenter = "Support: 815 59 043 &nbsp;&nbsp; Åpningstider: Man - fre: 09:00 - 19:00 (lørdag 10:00 - 17:00)";
    }
}

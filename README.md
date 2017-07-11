# MiniWGet
                       
Description: A small console app similar to WGet, this version only works with plain files. I'm coding this while I learn C# jejeje.
How to use:
Just type MiniWGet http://uri/path --param1:value1 --param2:value2
Parameters:
  --agent: Set user-agent, useful to spoof browsers.
  --cookie: Send specified cookies.
            Cookies should be sent in this format: COOKIE=VALUE;COOKIE2=VALUE2
            Useful to test hijacked sessions :)
  --referer: Set http-referer header.
  --help: Shows this help.
  --verbose: Turns on verbose mode
To save the file instead of display it, use > filename.ext (don't use this with verbose!!!)
Example: View Google's homepage:
        MiniWget https://google.com
Example2: Get Yandex's homepage and save it to a file:
        MiniWget https://yandex.ru > yandex.htm
Example3: Get Yandex's homepage sending a Cookie named PHPSESSID
        MiniWget https://yandex.ru --cookie:PHPSESSID=AAAAAAAAA
Folow me on Twitter: @underdog1987

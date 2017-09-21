# Black
Really scrappy HoloLens app to look at people, recognize them with Facebook's facial recognition, and pull up their name (like in the Black Mirror episode "Nosedive")

Small python server takes image uploads, adds them to your facebook account, finds friends tagged in the images, and returns the names. 

HoloLens app continually takes pictures and sends them to this server (not hosted anywhere, using ngrok.io) and uses the returned name to display. 

Shows that the premise works, but needs a lot of improvement - last updated June 2017

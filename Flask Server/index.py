import requests
import facebook
import time
import json
from flask import Flask, request

# needs some serious cleanup, error handling, etc. 

ALLOWED_EXTENSIONS = ['png', 'jpg']

token = 'YOUR_TOKEN'

graph = facebook.GraphAPI(access_token=token, version='2.4')

app = Flask(__name__)

def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

@app.route('/')
def root():
	return "nothin to see here..."

@app.route('/recognize', methods=['POST'])
def recognize():
    print 'request received'
    if 'file' not in request.files:
        return 'There was no file...'
    f = request.files['file']
    if f and allowed_file(f.filename):
        print 'image received'
        val = graph.put_photo(f)
        imgID = val['id']
        url = 'https://www.facebook.com/photos/tagging/recognition/?dpr=1.5'
        data = {'recognition_project':'composer_facerec',
                'photos[0]': imgID,
                'target':'',
                'is_page':'false',
                'include_unrecognized_faceboxes':'true',
                'include_face_crop_src':'true',
                'include_recognized_user_profile_picture':'false',
                'include_low_confidence_recognitions':'true',
                '__user':'100001913745575',
                '__a':'1',
                '__dyn':'5V4cjEzUGByC5A9UoHaEWy1mdhEK5EKiWFami8UNFLO6xG7UDAyoS2N6xCaxubwTCxKqEaUZ7yubkwy6UnGieKmrBKtojKeyohDWxaFQEd8HDBxe6rCCyVeFFUkgmUOfz8lUlwQxSayrhVo9ohzEKbwBxrxqrXm49aQ6EvGi5qh8gUKElCUmyE8XDh45EgAAxWqubAxxy8CXx6WK5bhEhUC8GeGqEOQifG12ByoB12labyEkybyVu',
                '__af':'iw',
                '__req':'27',
                '__be':'-1',
                '__pc':'PHASED:DEFAULT',
                '__rev':'3056542',
                'fb_dtsg':'AQF0czUFZQGH:AQE8LZ9yLuSq',
                'logging':'2658170489912285709081717258658169567690571217611783113'
                }
        headers = {'x_fb_background_state': '1',
                'origin': 'https://www.facebook.com',
                'accept-encoding': 'gzip, deflate, lzma',
                'accept-language': 'en-US,en;q=0.8',
                'user-agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36',
                'content-type': 'application/x-www-form-urlencoded',
                'accept': '*/*',
                'referer': 'https://www.facebook.com/',
                'cookie': 'sb=1PcGV92rZeIf2xRv_qE0s1mh; lu=TguRfSmue_mOfgKcZ1f9DhKA; datr=azH2VfiK4oNk_qYrm4oHauvn; dats=1; c_user=100001913745575; xs=13%3A4E0G9piNBKf0DQ%3A2%3A1484373969%3A7322; fr=05qLut2mNH89ebE9k.AWXxeknbR-lITnLckWcvimT8DC0.BV9jGA.-U.Fko.0.0.BZL3yG.AWVr76C4; act=1496289191713%2F13; presence=EDvF3EtimeF1496289206EuserFA21B01913745575A2EstateFDutF1496289206693CEchFDp_5f1B01913745575F44CC'
                }
        print 'about to sleep'
        time.sleep(3)

        ans = requests.post(url, data=data, headers=headers)
        print 'about to send response'
        j = json.loads(ans.content[9:])
        print ans.content
        print j
        faces = j['payload'][0]['faceboxes']
        if len(faces) == 0: 
            return 'Nobody Recognized'
        name = faces[0]['recognitions'][0]['user']['name']
        return name
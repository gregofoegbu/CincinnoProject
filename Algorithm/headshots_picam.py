import cv2
from picamera import PiCamera
from picamera.array import PiRGBArray
from pathlib import Path


def headshotCapturePi():

    name = input('What is your name?\n')  # replace with your name

    cam = PiCamera()
    cam.resolution = (1296, 976)
    cam.framerate = 15
    rawCapture = PiRGBArray(cam, size=(1296, 976))

    img_counter = 0

    while True:
        for frame in cam.capture_continuous(rawCapture, format="bgr", use_video_port=True):
            image = frame.array
            image = cv2.resize(image, (512, 304))
            cv2.imshow("Press Space to take a photo", image)
            rawCapture.truncate(0)

            k = cv2.waitKey(1)
            rawCapture.truncate(0)
            if k % 256 == 27:  # ESC pressed
                break
            elif k % 256 == 32:
                # SPACE pressed
                Path("dataset/" + name).mkdir(parents=True, exist_ok=True)
                img_name = "dataset/" + name + "/image_{}.jpg".format(img_counter)
                cv2.imwrite(img_name, image)
                print("{} written!".format(img_name))
                img_counter += 1

        if k % 256 == 27:
            print("Escape hit, closing...")
            break

    cv2.destroyAllWindows()
    cam.stop_preview()
    cam.close()
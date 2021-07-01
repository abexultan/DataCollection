
import random
from PIL import Image, ImageDraw
import os


if __name__ == '__main__':

    DATA_FOLDER = './'
    screen_w = 1920  # Change to your screen resolution
    screen_h = 1080
    images = os.listdir(DATA_FOLDER + 'images/')
    labels = os.listdir(DATA_FOLDER + 'labels/')
    idx_rand = random.randint(0, len(images) - 1)

    # 0 - cars, 1 - trucks, 2 - humans, 3 - scooters
    colors = {0: 'red', 1: 'green', 2: 'blue', 3: 'white'}

    random_image = Image.open(DATA_FOLDER + f'images/{images[idx_rand]}')
    with open(DATA_FOLDER + f'labels/{labels[idx_rand]}', 'r') as f:
        lines = f.readlines()

    lines = list(map(lambda x: x.strip().split(), lines))
    draw = ImageDraw.Draw(random_image)

    for line in lines:
        cls_id, center_x, center_y, width, height = line
        x0 = int(float(center_x) * screen_w - (float(width) * screen_w) / 2)
        x1 = int(float(center_x) * screen_w + (float(width) * screen_w) / 2)
        y0 = int(float(center_y) * screen_h - (float(height) * screen_h) / 2)
        y1 = int(float(center_y) * screen_h + (float(height) * screen_h) / 2)
        draw.rectangle((x0, y0, x1, y1), outline=colors[int(cls_id)])

    random_image.show()

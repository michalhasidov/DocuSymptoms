
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager


import sys
#arg1 = sys.argv[1]
# with open("C:\\Users\\user nc\\Desktop\\קודים ניסיונים לפרויקט\\root.txt", "r", encoding="utf-8") as f:
#     arg1 = f.readline()
def root(verb):
    url = "https://hebrew-academy.org.il/%d7%9c%d7%95%d7%97%d7%95%d7%aa-%d7%a0%d7%98%d7%99%d7%99%d7%aa-%d7%94%d7%a4%d7%95%d7%a2%d7%9c/?action=nituah&poal="
    #driver = webdriver.Chrome("C:\\Users\\user nc\\Desktop\\myexel\\chromedriver\\chromedriver.exe")
    options = webdriver.ChromeOptions()
    options.add_argument('--headless')
    driver = webdriver.Chrome(ChromeDriverManager(version="116.0.5845.141").install())

    driver.get(url)
    button = driver.find_element(By.ID, "hataya")
    button.clear()
    button.send_keys(verb)
    time.sleep(5)
    try:
        tr = driver.find_elements(By.TAG_NAME, "tr")[1]
        td = driver.find_elements(By.TAG_NAME, "td")[2]

        with open("C:\\Users\\user nc\\Desktop\\קודים ניסיונים לפרויקט\\root.txt", "w", encoding="utf-8") as f:
            f.write(td.text)
        print(td.text)

    except:
        error = driver.find_element(By.CLASS_NAME, "poal-table-container")
        print(error.text)

root("שמרתי")

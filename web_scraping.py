
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.service import Service
def webScraping(diseases):

    url = "https://www.infomed.co.il/diseases/"
    #driver = webdriver.Chrome("C:/Users/user nc/Desktop/myexel/chromedriver/chromedriver.exe")
    options = webdriver.ChromeOptions()
    options.add_argument('--headless')
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=options)
    driver.get(url)
    search_button = driver.find_element(By.ID, "freeText")
    search_button.clear()
    search_button.send_keys(diseases)

    time.sleep(1)
    d = driver.find_element(By.ID, "searchResultsContainer").find_elements(By.TAG_NAME, "a")[0]
    d.click()
    time.sleep(3)
    description = driver.find_elements(By.CLASS_NAME, "description")[1]
    print(description.text)
    drawers_list = driver.find_element(By.CLASS_NAME, "drawers_list").find_elements(By.TAG_NAME, "li")
    print(drawers_list[0].text)
    for i in drawers_list:
        b = i.find_element(By.CLASS_NAME, "signIcon")
        b.click()
        time.sleep(5)

        drawerExpanded = i.find_element(By.CLASS_NAME, "drawer_expanded")
        # pDrawerExpanded = drawerExpanded.find_elements(By.TAG_NAME, "p")
        try:
            print(i.find_element(By.TAG_NAME, "h2").text)

        except:
            print("nofound")

        print(drawerExpanded.text)


    addInformatiom = driver.find_element(By.CLASS_NAME, "drug_relativeContentList").find_elements(By.TAG_NAME,"li")
    for i in addInformatiom:
        print(i.text)
















import requests
import json
import time
import pandas

edgexip = '127.0.0.1'
ph = list()
Organic_carbon = list()
Turbidity = list()


def readSensorData():
    data = pandas.read_csv("water_potability.csv")

    global ph
    global Organic_carbon
    global Turbidity

    ph = data["ph"].tolist()
    Organic_carbon = data["Organic_carbon"].tolist()
    Turbidity = data["Turbidity"].tolist()


if __name__ == "__main__":
    readSensorData()
    i = 0
    while(i < len(ph)):

        url = 'http://%s:49986/api/v1/resource/water_potability_sensor_cluster_01/ph' % edgexip
        payload = ph[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        url = 'http://%s:49986/api/v1/resource/water_potability_sensor_cluster_01/Organic_carbon' % edgexip
        payload = Organic_carbon[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        url = 'http://%s:49986/api/v1/resource/water_potability_sensor_cluster_01/Turbidity' % edgexip
        payload = Turbidity[i]
        headers = {'content-type': 'application/json'}
        response = requests.post(url, data=json.dumps(
            payload), headers=headers, verify=False)

        i = i+1
        print(i)
        time.sleep(5)

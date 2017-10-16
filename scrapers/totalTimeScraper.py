class BoxInfo:
    toteId = ""
    orderId = ""
    time = ""

    def __init__(self, toteId, orderId, time):
        self.toteId = toteId
        self.orderId = orderId
        self.time = time

    def __str__(self):
        return "ToteId: " + self.toteId + "\nOrderId: " + self.orderId + "\nTime: " + self.time


def fixOrderId(orderId):
    return orderId[2:-1]

counter = 0
filne = "ErpTask_trace.log"
with open(filne, 'r') as f:
    lines = f.readlines()
    for i in range(0, len(lines)):
        line = lines[i]
        if line[:7] == "ToteId:":
            counter = counter + 1
            time = lines[i-4][:23].strip()
            orderNum = fixOrderId(lines[i+1][13:].strip())
            toteId = line[7:].strip()
            box = BoxInfo(toteId, orderNum, time)
            print(box)
            print()
            #ne = lines[i + 1] # you may want to check that i < len(lines)
            #print ' ne ',ne,'\n'
            #break


print("COUNT: " + str(counter))


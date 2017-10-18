import string
from typing import *
import datetime
import  random
import time

def checkTypeReturn(input, expectedType):
    if isinstance(input, expectedType):
        return input
    else:
        raise TypeError(str(input) + " is of type " + str(type(input)) + " expected: " + str(expectedType))


class ArticleNumber:
    def __init__(self, articleNum : int):
        self.articleNumber = checkTypeReturn(articleNum, int)
        if 10000000 > self.articleNumber or self.articleNumber > 9999999999:
            raise ValueError("Invalid article number entered.")

    def __str__(self) -> str:
        return str(self.articleNumber)


class ItemType:
    totalProcessTime = datetime.time
    def __init__(self, articleNum: ArticleNumber):
        self.articleNumber = checkTypeReturn(articleNum, ArticleNumber)

    def __str__(self) -> str:
        return str(self.articleNumber)


class Line:
    def __init__(self, itemType : ItemType, count : int):
        self.itemType = checkTypeReturn(itemType, ItemType)
        self.count = checkTypeReturn(count, int)

    def __str__(self) -> str:
        return "(%s, %s)" % (str(self.itemType), str(self.count))


class Order:
    def __init__(self, orderName : str, timeToFinish : int):
        self.orderName = orderName
        self.timeToFinish = timeToFinish

    def __str__(self) -> str:
        return "(%s, %s)" % (self.orderName, self.timeToFinish)

class OrderBox:
    def __init__(self, order : Order ):
        self.order = order
        self.timeRemaining = order.timeToFinish

    def __str__(self):
        return "(%s, %s)" % (self.order, self.timeRemaining)


def GenerateOrders (amount : int):
    #Generates and returns a list of Orders, with random identifiers and processing times.
    orders = []

    for x in range(0, amount):
        orders.append(Order(random.choice(string.ascii_letters.upper()), random.randint(10, 100)))
    return orders


def getNext():
    return GenerateOrders(1)

class System:
    def __init__(self, maxConcurrentItems : int):
        self.orderList = orderList
        self.finishedOrders = [Order]
        self.maxConcurrentItems = maxConcurrentItems
        self.orderBoxList: list(Order) = []

    def initializeOrderBoxes(self):
        # for hver ordre i orderlist:
        for order in self.orderList:
            # opret ny orderbox med order og tid fra ordren og tilføj den nye orderbox til orderboxlist
            self.orderBoxList.append(OrderBox(order))

    def __str__(self):
        return "Order Boxes: %s, Orders Finished: %s\n" % (len(self.orderBoxList), len(self.finishedOrders))
        """
        myStr = "Order Boxes: \n"

        for order in self.orderBoxList:
            myStr += str(order) + "\n"
        return myStr
        """

    def Update(self):
        if(len(self.orderBoxList) < self.maxConcurrentItems):
            self.orderBoxList.append(getNext())
            for orderBox in self.orderBoxList:
                orderBox.timeRemaining -= 1
                if(orderBox.timeRemaining == 0):
                    self.finishedOrders.append(orderBox.order)
                    self.orderBoxList.remove(orderBox)



print("START STATE")

system = System(3)

for i in range(100):
    system.Update()
    time.sleep(2)
    print(system)


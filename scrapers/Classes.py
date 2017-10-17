from typing import *
import datetime

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


class System:
    def __init__(self, orderList, maxConcurrentItems : int):
        self.orderList = orderList
        self.finishedOrders = [Order]
        self.maxConcurrentItems = maxConcurrentItems
        self.orderBoxList = [OrderBox]

    def initializeOrderBoxes(self):
        # Lav en orderboxlist
        self.orderBoxList = [OrderBox]

        # for hver ordre i orderlist:
        for order in self.orderList:
            # opret ny orderbox med order og tid fra ordren
            newOrderBox = OrderBox(order)

            # tilføj den nye orderbox til orderboxlist
            self.orderBoxList.append(newOrderBox)

    def __str__(self):
        myStr = ("My Order Boxes:\n")
        for order in self.orderBoxList:
            myStr += str(order) + "\n"
        return myStr


myOrders = [Order("A", 1), Order("B", 21), Order("C", 12), Order("D", 41)]

system = System(myOrders, 3)
system.initializeOrderBoxes()

print(system)







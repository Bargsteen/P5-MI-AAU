import string
from typing import *
import datetime
import random
import time
import heapq


def checkTypeReturn(input, expectedType):
    if isinstance(input, expectedType):
        return input
    else:
        raise TypeError(str(input) + " is of type " + str(type(input)) + " expected: " + str(expectedType))


class ArticleNumber:
    def __init__(self, articleNum: int):
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
    def __init__(self, itemType: ItemType, count: int):
        self.itemType = checkTypeReturn(itemType, ItemType)
        self.count = checkTypeReturn(count, int)

    def __str__(self) -> str:
        return "(%s, %s)" % (str(self.itemType), str(self.count))


class Order:
    def __init__(self, orderName: str, timeToFinish: int):
        self.orderName = orderName
        self.timeToFinish = timeToFinish

    def __str__(self) -> str:
        return "(%s, %s)" % (self.orderName, self.timeToFinish)


class OrderBox:
    def __init__(self, order: Order):
        self.order = order
        self.timeRemaining = order.timeToFinish

    def __str__(self):
        return "(%s, %s)" % (self.order, self.timeRemaining)


def GenerateOrders(amount: int):
    # Generates and returns a list of Orders, with random identifiers and processing times.
    orders = []

    for x in range(0, amount):
        orders.append(Order(random.choice(string.ascii_letters.upper()), random.randint(10, 100)))
    return orders


def getNext():
    return GenerateOrders(1)[0]


class System:
    def __init__(self, maxConcurrentItems: int):
        self.finishedOrders: list(Order) = []
        self.maxConcurrentItems = maxConcurrentItems
        self.orderBoxList: list(OrderBox) = []
        self.totalTimeRun = 0

    def initializeOrderBoxes(self):
        # for hver ordre i orderlist:
        for order in self.orderList:
            # opret ny orderbox med order og tid fra ordren og tilføj den nye orderbox til orderboxlist
            self.orderBoxList.append(OrderBox(order))

    def __str__(self):
        header = "------- Time: %s ----- Order Boxes in System: %s, Orders Finished: %s\n" % (
        self.totalTimeRun, len(self.orderBoxList), len(self.finishedOrders))
        orderBoxList = "Order Boxes in System:\n" + str([str(o) for o in self.orderBoxList]) + "\n"
        finishedOrders = "Finished Orders:\n" + str([str(o) for o in self.finishedOrders]) + "\n"
        return header + orderBoxList + finishedOrders

    def update(self):
        self.totalTimeRun += 1
        if len(self.orderBoxList) < self.maxConcurrentItems:
            self.orderBoxList.append(OrderBox(getNext()))
        for orderBox in self.orderBoxList:
            orderBox.timeRemaining -= 1
            if orderBox.timeRemaining == 0:
                self.finishedOrders.append(orderBox.order)
                self.orderBoxList.remove(orderBox)


system = System(10)

#print('START STATE')
#print(system)

a = Order("QQ", 1)
b = Order("A", 100)
c = Order("C", 2)


class MyHeap(object):
   def __init__(self, initial=None, key=lambda x:x):
       self.key = key
       if initial:
           self._data = [(key(item), item) for item in initial]
           heapq.heapify(self._data)
       else:
           self._data = []

   def push(self, item):
       heapq.heappush(self._data, (self.key(item), item))

   def pop(self):
       return heapq.heappop(self._data)[1]


myHeap = MyHeap(key=lambda x: x.timeToFinish)
myHeap.push(b)
myHeap.push(a)
myHeap.push(c)

print(myHeap.pop())
print(myHeap.pop())
print(myHeap.pop())
"""
for i in range(100):
    system.update()
    time.sleep(0.001)
    print(system)
"""


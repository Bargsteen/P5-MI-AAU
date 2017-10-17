import string
from typing import *
import datetime
import  random

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


def GenerateOrders (amount : int):
    #Generates and returns a list of Orders, with random identifiers and processing times.
    orders = []

    for x in range(0, amount):
        orders.append(Order(random.choice(string.ascii_letters.upper()), random.randint(10, 1000)))
    return orders


orders = GenerateOrders(100)

for x in range(0, len(orders)):
    print(orders[x])

myOrder = Order("hej2", 321)
print(myOrder.__str__())







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
    def __str__(self):
        return str(self.articleNumber)

class ItemType:
    totalProcessTime = datetime.time
    def __init__(self, articleNum: ArticleNumber):
        self.articleNumber = checkTypeReturn(articleNum, ArticleNumber)

    def __str__(self):
        return str(self.articleNumber)


class Line:
    def __init__(self, itemType : ItemType, count : int):
        self.itemType = checkTypeReturn(itemType, ItemType)
        self.count = checkTypeReturn(count, int)

    def __str__(self):
        return "(%s, %s)" % (str(self.itemType), str(self.count))

art = ArticleNumber(232323232)
itemTy = ItemType(art)
line = Line(itemTy, 1234)

print(line)

#class Order:
 #   timeOfReceival = datetime.datetime
   # orderId = ""
  #  lines = [(ItemType(), 0)]
import csv
import typing

class Article:
    def __init__(self, articleNumber, timeStamp, startArea):
        self.articleNumber : str = articleNumber
        self.timeStamp : str = timeStamp
        self.startArea : str = startArea
    
    def __str__(self):
        return "\t ArticleNumber: " + self.articleNumber + "\n\t TimeStamp: " + self.timeStamp + "\n\t StartArea: " + self.startArea + "\n"

class Order:
    def __init__(self, orderNumber, article):
        self.orderNumber : str = orderNumber
        self.articles : list = []
        self.articles.append(article)
    
    def appendArticle(self, article):
        self.articles.append(article)
    
    def __str__(self):
        articlesString : str = ""
        for i, art in enumerate(self.articles):
            articlesString += str(str(i+1) + str(art)) + "\n"
        return "Ordernumber: " + self.orderNumber + "\n" + articlesString
        
pickingPath : str = "~/Downloads/wetransfer-f8286e/"
pickingSource : str = "Picking 02-10-2017.csv"
pickingFilepath : str = pickingPath + pickingSource

erpTracePath : str = "~/Downloads/wetransfer-f8286e/"
erpTraceSource : str = "ErpTask_trace.log"
erpTraceFilepath : str = erpTracePath + erpTraceSource

# Read all orders + their associated times and articles. 
pickingReader : str = csv.reader(open(pickingFilepath, newline=''), delimiter = ';', quotechar='|')
next(pickingReader, None)

pickingOrdersList : list = []
erptraceOrderList : list = []

# Iterate through picking 02... file, and retrieve orders.
for i, row in enumerate(pickingReader):
    if(i > 0 and row[0] == pickingOrdersList[-1].orderNumber):
        pickingOrdersList[-1].appendArticle(Article(row[1], row[8], row[10]))
    else:
        pickingOrdersList.append(Order(row[0], Article(row[1], row[8], row[10])))


# Iterate through erptrace.log... file, and retrieve orders.

# Print stuff
for order in pickingOrdersList:
    print(order)


"""
# Time[10:18]
"""
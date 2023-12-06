# Synapse Analytics notebook source

# METADATA ********************

# META {
# META   "synapse": {
# META     "lakehouse": {
# META       "default_lakehouse": "{LAKEHOUSE_ID}",
# META       "default_lakehouse_name": "{LAKEHOUSE_NAME}",
# META       "default_lakehouse_workspace_id": "{WORKSPACE_ID}",
# META       "known_lakehouses": [
# META         {
# META           "id": "{LAKEHOUSE_ID}"
# META         }
# META       ]
# META     }
# META   }
# META }

# CELL ********************

# copy CSV files to lakehouse to load data into bronze layer 
import requests

csv_base_url = "https://github.com/PowerBiDevCamp/ProductSalesData/raw/main/"

csv_files = { "Customers.csv", "Products.csv", "Invoices.csv", "InvoiceDetails.csv" }

folder_path = "Files/bronze_landing_layer/"

for csv_file in csv_files:
    csv_file_path = csv_base_url + csv_file
    with requests.get(csv_file_path) as response:
        csv_content = response.content.decode('utf-8-sig')
        mssparkutils.fs.put(folder_path + csv_file, csv_content, True)
        print(csv_file + " copied to Lakehouse file in OneLake")

# CELL ********************

# create products table for silver layer
from pyspark.sql.types import StructType, StructField, StringType, LongType, FloatType

# create schema for products table using StructType and StructField 
schema_products = StructType([
    StructField("ProductId", LongType() ),
    StructField("Product", StringType() ),
    StructField("Category", StringType() )
])

# Load CSV file into Spark DataFrame and validate data using schema
df_products = (
    spark.read.format("csv")
         .option("header","true")
         .schema(schema_products)
         .load("Files/bronze_landing_layer/Products.csv")
)

# save DataFrame as lakehouse table in Delta format
( df_products.write
             .mode("overwrite")
             .option("overwriteSchema", "True")
             .format("delta")
             .save("Tables/silver_products")
)

# display table schema and data
df_products.printSchema()
df_products.show()

# CELL ********************

# create customers table for silver layer
from pyspark.sql.types import StructType, StructField, StringType, LongType, DateType

# create schema for customers table using StructType and StructField 
schema_customers = StructType([
    StructField("CustomerId", LongType() ),
    StructField("FirstName", StringType() ),
    StructField("LastName", StringType() ),
    StructField("Country", StringType() ),
    StructField("City", StringType() ),
    StructField("DOB", DateType() ),
])

# Load CSV file into Spark DataFrame with schema and support to infer dates
df_customers = (
    spark.read.format("csv")
         .option("header","true")
         .schema(schema_customers)
         .option("dateFormat", "MM/dd/yyyy")
         .option("inferSchema", "true")
         .load("Files/bronze_landing_layer/Customers.csv")
)

# save DataFrame as lakehouse table in Delta format
( df_customers.write
              .mode("overwrite")
              .option("overwriteSchema", "True")
              .format("delta")
              .save("Tables/silver_customers")
)

# display table schema and data
df_customers.printSchema()
df_customers.show()

# CELL ********************

# create invoices table for silver layer
from pyspark.sql.types import StructType, StructField, LongType, FloatType, DateType

# create schema for invoices table using StructType and StructField 
schema_invoices = StructType([
    StructField("InvoiceId", LongType() ),
    StructField("Date", DateType() ),
    StructField("TotalSalesAmount", FloatType() ),
    StructField("CustomerId", LongType() )
])

# Load CSV file into Spark DataFrame with schema and support to infer dates
df_invoices = (
    spark.read.format("csv")
         .option("header","true")
         .schema(schema_invoices)
         .option("dateFormat", "MM/dd/yyyy")
         .option("inferSchema", "true") 
         .load("Files/bronze_landing_layer/Invoices.csv")
)

# save DataFrame as lakehouse table in Delta format
( df_invoices.write
             .mode("overwrite")
             .option("overwriteSchema", "True")
             .format("delta")
             .save("Tables/silver_invoices")
)

# display table schema and data
df_invoices.printSchema()
df_invoices.show()

# CELL ********************

# create invoice_details table for silver layer
from pyspark.sql.types import StructType, StructField, LongType, FloatType

# create schema for invoice_details table using StructType and StructField 
schema_invoice_details = StructType([
    StructField("Id", LongType() ),
    StructField("Quantity", LongType() ),
    StructField("SalesAmount", FloatType() ),
    StructField("InvoiceId", LongType() ),
    StructField("ProductId", LongType() )
])

# Load CSV file into Spark DataFrame and validate data using schema
df_invoice_details = (
    spark.read.format("csv")
         .option("header","true")
         .schema(schema_invoice_details)
         .load("Files/bronze_landing_layer/InvoiceDetails.csv")
)

# save DataFrame as lakehouse table in Delta format
( df_invoice_details.write
                    .mode("overwrite")
                    .option("overwriteSchema", "True")
                    .format("delta")
                    .save("Tables/silver_invoice_details")
)

# display table schema and data
df_invoice_details.printSchema()
df_invoice_details.show()

# CELL ********************

# create products table for gold layer

# load DataFrame from silver layer table
df_gold_products = (
    spark.read
         .format("delta")
         .load("Tables/silver_products")
)

# write DataFrame to new gold layer table 
( df_gold_products.write
                  .mode("overwrite")
                  .option("overwriteSchema", "True")
                  .format("delta")
                  .save("Tables/products")
)

# display table schema and data
df_gold_products.printSchema()
df_gold_products.show()

# CELL ********************

# create customers table for gold layer
from pyspark.sql.functions import concat_ws, floor, datediff, current_date, col

# load DataFrame from silver layer table and perform transforms
df_gold_customers = (
    spark.read
         .format("delta")
         .load("Tables/silver_customers")
         .withColumn("Customer", concat_ws(' ', col('FirstName'), col('LastName')) )
         .withColumn("Age",( floor( datediff( current_date(), col("DOB") )/365.25) ))   
         .drop('FirstName', 'LastName')
)

# write DataFrame to new gold layer table 
( df_gold_customers.write
                   .mode("overwrite")
                   .option("overwriteSchema", "True")
                   .format("delta")
                   .save("Tables/customers")
)

# display table schema and data
df_gold_customers.printSchema()
df_gold_customers.show()

# CELL ********************

# create sales table for gold layer
from pyspark.sql.functions import col, desc, concat, lit, floor, datediff
from pyspark.sql.functions import date_format, to_date, current_date, year, month, dayofmonth

# load DataFrames using invoices table and invoice_details table from silver layer
df_silver_invoices = spark.read.format("delta").load("Tables/silver_invoices")
df_silver_invoice_details = spark.read.format("delta").load("Tables/silver_invoice_details")

# perform join to merge columns from both DataFrames and transform data 
df_gold_sales = (
    df_silver_invoice_details
        .join(df_silver_invoices, 
              df_silver_invoice_details['InvoiceId'] == df_silver_invoices['InvoiceId'])
        .withColumnRenamed('SalesAmount', 'Sales')
        .withColumn("DateKey", (year(col('Date'))*10000) + 
                               (month(col('Date'))*100) + 
                               (dayofmonth(col('Date')))   )
        .drop('InvoiceId', 'TotalSalesAmount', 'InvoiceId', 'Id')
        .select('Date', "DateKey", "CustomerId", "ProductId", "Sales", "Quantity")
)

# write DataFrame to new gold layer table 
( df_gold_sales.write
               .mode("overwrite")
               .option("overwriteSchema", "True")
               .format("delta")
               .save("Tables/sales")
)

# display table schema and data
df_gold_sales.printSchema()
df_gold_sales.show()

# CELL ********************

# create calendar table for gold layer
from datetime import date
import pandas as pd
from pyspark.sql.functions import to_date, year, month, dayofmonth, quarter, dayofweek, date_format

# get first and last calendar date from sakes table 
first_sales_date = df_gold_sales.agg({"Date": "min"}).collect()[0][0]
last_sales_date = df_gold_sales.agg({"Date": "max"}).collect()[0][0]

# calculate start date and end date for calendar table
start_date = date(first_sales_date.year, 1, 1)
end_date = date(last_sales_date.year, 12, 31)

# create pandas DataFrame with Date series column
df_calendar_ps = pd.date_range(start_date, end_date, freq='D').to_frame()

# convert pandas DataFrame to Spark DataFrame and add calculated calendar columns
df_calendar_spark = (
     spark.createDataFrame(df_calendar_ps)
       .withColumnRenamed("0", "timestamp")
       .withColumn("Date", to_date(col('timestamp')))
       .withColumn("DateKey", (year(col('timestamp'))*10000) + 
                              (month(col('timestamp'))*100) + 
                              (dayofmonth(col('timestamp')))   )
       .withColumn("Year", year(col('timestamp'))  )
       .withColumn("Quarter", date_format(col('timestamp'),"yyyy-QQ")  )
       .withColumn("Month", date_format(col('timestamp'),'yyyy-MM')  )
       .withColumn("Day", dayofmonth(col('timestamp'))  )
       .withColumn("MonthInYear", date_format(col('timestamp'),'MMMM')  )
       .withColumn("MonthInYearSort", month(col('timestamp'))  )
       .withColumn("DayOfWeek", date_format(col('timestamp'),'EEEE')  )
       .withColumn("DayOfWeekSort", dayofweek(col('timestamp')))
       .drop('timestamp')
)

# write DataFrame to new gold layer table 
( df_calendar_spark.write
                   .mode("overwrite")
                   .option("overwriteSchema", "True")
                   .format("delta")
                   .save("Tables/calendar")
)

# display table schema and data
df_calendar_spark.printSchema()
df_calendar_spark.show()

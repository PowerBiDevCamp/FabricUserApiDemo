from datetime import date
import pandas as pd
from pyspark.sql.functions import to_date, year, month, dayofmonth, dayofweek, date_format, current_date
from pyspark.sql.functions import concat_ws, floor, datediff, current_date, col

from pyspark.sql import SparkSession
spark = SparkSession.builder.appName("abc").getOrCreate()

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

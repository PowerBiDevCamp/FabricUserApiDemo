# create products table for silver layer
from pyspark.sql.types import StructType, StructField, StringType, LongType, FloatType, DateType

from pyspark.sql import SparkSession
spark = SparkSession.builder.appName("abc").getOrCreate()

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
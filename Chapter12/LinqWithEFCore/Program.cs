﻿using System;
using static System.Console;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LinqWithEFCore
{
    class Program
    {
        static void FilterAndSort()
        {
            using (var db = new Northwind())
            {
                var query = db.Products
                // query is a DbSet<Product>
                .Where(product => product.UnitPrice < 10M)
                // query is now n IQueryable<Product>
                .OrderByDescending(product => product.UnitPrice)
                .Select(product => new //anonymous type
                {
                    product.ProductID,
                    product.ProductName,
                    product.UnitPrice
                });

                WriteLine(query.ToQueryString());// How to view query string. 

                WriteLine("Products that cost less than $10: ");
                foreach (var item in query)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} cost {item.UnitPrice: $#,##0.00}");
                }
                WriteLine();
            }
        }

        static void JoinCategoriesAndProducts()
        {
            using (var db = new Northwind())
            {
                // join every product to its category to return 77 matches
                var queryJoin = db.Categories.Join(
                    inner: db.Products,
                    outerKeySelector: category => category.CategoryID,
                    innerKeySelector: product => product.CategoryID,
                    resultSelector: (c, p) => new { c.CategoryName, p.ProductName, p.ProductID })
                    .OrderBy(cp => cp.CategoryName);

                WriteLine(queryJoin.ToQueryString());

                foreach (var item in queryJoin)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} is in {item.CategoryName}");
                }

            }
        }

        static void GroupJoinCategoriesAndProducts()
        {
            using (var db = new Northwind())
            {
                // group all products by their category to reutnr 8 matches
                var queryGroup = db.Categories.AsEnumerable().GroupJoin(//AsEnumerable() prevents system.NotImplementedException
                    inner: db.Products,
                    outerKeySelector: category => category.CategoryID,
                    innerKeySelector: product => product.CategoryID,
                    resultSelector: (c, matchingProducts) => new
                    {
                        c.CategoryName,
                        Products = matchingProducts.OrderBy(p => p.ProductName)
                    });

                foreach (var item in queryGroup)
                {
                    WriteLine($"{item.CategoryName} has {item.Products.Count()}");

                    foreach (var product in item.Products)
                    {
                        WriteLine($" {product.ProductName}");
                    }
                }

                
            }

        }

        static void AggregateProducts()
        {
            using (var db = new Northwind())
            {
                WriteLine($"{"Products Count:", -25} {db.Products.Count()}");
                WriteLine($"{"Highest product price:", -25} {db.Products.Max(p => p.UnitPrice)}");
                WriteLine($"{"Sum of units in stock:", -25} {db.Products.Sum(p => p.UnitsInStock)}");
                WriteLine($"{"Sum of units on order:", -25} {db.Products.Sum(p => p.UnitsOnOrder)}");
                WriteLine($"{"Average unit price: ", -25} {db.Products.Average(p => p.UnitPrice), 10:$#,##0.00}");
                WriteLine($"{"Value of units in stock", -25} {db.Products.AsEnumerable().Sum(p => p.UnitPrice * p.UnitsInStock)}");
            }
        }

        static void Main()
        {
            //FilterAndSort();
            //JoinCategoriesAndProducts();
            //GroupJoinCategoriesAndProducts();
            AggregateProducts();
        }
    }
}

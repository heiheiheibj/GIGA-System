using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// OutboundOrderHelper_Inventory 的摘要说明
/// </summary>
public partial class OutboundOrderHelper
{
    #region 库存相关方法

    /// <summary>
    /// 更新库存
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="productId">产品ID</param>
    /// <param name="shelfId">货架ID</param>
    /// <param name="quantity">数量（正数为增加，负数为减少）</param>
    /// <param name="batchNumber">批次号</param>
    /// <param name="expiryDate">过期日期</param>
    /// <returns>返回结果对象，包含成功标志和消息</returns>
    public static object UpdateInventory(int warehouseId, int productId, int shelfId, decimal quantity, string batchNumber, DateTime? expiryDate)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = ""
        };

        // 参数验证
        if (warehouseId <= 0)
        {
            return new { success = false, message = "仓库ID无效" };
        }

        if (productId <= 0)
        {
            return new { success = false, message = "产品ID无效" };
        }

        if (shelfId <= 0)
        {
            return new { success = false, message = "货架ID无效" };
        }

        try
        {
            // 创建SQL参数列表
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WarehouseID", warehouseId));
            parameters.Add(new SqlParameter("@ProductID", productId));
            parameters.Add(new SqlParameter("@ShelfID", shelfId));
            parameters.Add(new SqlParameter("@Quantity", quantity));
            parameters.Add(new SqlParameter("@BatchNumber", batchNumber ?? (object)DBNull.Value));

            // 处理过期日期
            if (expiryDate.HasValue)
            {
                parameters.Add(new SqlParameter("@ExpiryDate", expiryDate.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@ExpiryDate", DBNull.Value));
            }

            // 添加输出参数
            SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            parameters.Add(outputParam);

            // 执行存储过程
            int rowsAffected = SqlHelper.ExecuteNonQuery("sp_Inventory_Update", parameters.ToArray());

            // 获取输出参数值
            int updateResult = Convert.ToInt32(outputParam.Value);

            // 根据结果返回不同的消息
            if (updateResult == 1)
            {
                result = new
                {
                    success = true,
                    message = "库存更新成功"
                };
            }
            else if (updateResult == -1)
            {
                result = new
                {
                    success = false,
                    message = "库存不足，无法减少"
                };
            }
            else
            {
                result = new
                {
                    success = false,
                    message = "库存更新失败"
                };
            }
        }
        catch (Exception ex)
        {
            // 异常处理
            result = new
            {
                success = false,
                message = "库存更新失败：" + ex.Message
            };
        }

        return result;
    }

    /// <summary>
    /// 添加库存日志
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="productId">产品ID</param>
    /// <param name="shelfId">货架ID</param>
    /// <param name="quantity">数量（正数为增加，负数为减少）</param>
    /// <param name="operationType">操作类型（1：入库，2：出库，3：调整）</param>
    /// <param name="sourceId">来源ID（如入库单ID、出库单ID等）</param>
    /// <param name="sourceType">来源类型（1：入库单，2：出库单，3：盘点单）</param>
    /// <param name="batchNumber">批次号</param>
    /// <param name="expiryDate">过期日期</param>
    /// <param name="remark">备注</param>
    /// <returns>返回结果对象，包含成功标志和消息</returns>
    public static object InsertInventoryLog(int warehouseId, int productId, int shelfId, decimal quantity, int operationType, int sourceId, int sourceType, string batchNumber, DateTime? expiryDate, string remark)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = ""
        };

        // 参数验证
        if (warehouseId <= 0)
        {
            return new { success = false, message = "仓库ID无效" };
        }

        if (productId <= 0)
        {
            return new { success = false, message = "产品ID无效" };
        }

        if (shelfId <= 0)
        {
            return new { success = false, message = "货架ID无效" };
        }

        try
        {
            // 创建SQL参数列表
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@WarehouseID", warehouseId));
            parameters.Add(new SqlParameter("@ProductID", productId));
            parameters.Add(new SqlParameter("@ShelfID", shelfId));
            parameters.Add(new SqlParameter("@Quantity", quantity));
            parameters.Add(new SqlParameter("@OperationType", operationType));
            parameters.Add(new SqlParameter("@SourceID", sourceId));
            parameters.Add(new SqlParameter("@SourceType", sourceType));
            parameters.Add(new SqlParameter("@BatchNumber", batchNumber ?? (object)DBNull.Value));

            // 处理过期日期
            if (expiryDate.HasValue)
            {
                parameters.Add(new SqlParameter("@ExpiryDate", expiryDate.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@ExpiryDate", DBNull.Value));
            }

            parameters.Add(new SqlParameter("@Remark", remark ?? (object)DBNull.Value));

            // 添加输出参数
            SqlParameter outputParam = new SqlParameter("@InventoryLogID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            parameters.Add(outputParam);

            // 执行存储过程
            int rowsAffected = SqlHelper.ExecuteNonQuery("sp_InventoryLog_Insert", parameters.ToArray());

            // 获取输出参数值
            int inventoryLogId = Convert.ToInt32(outputParam.Value);

            // 返回结果
            result = new
            {
                success = true,
                message = "库存日志添加成功",
                inventoryLogId = inventoryLogId
            };
        }
        catch (Exception ex)
        {
            // 异常处理
            result = new
            {
                success = false,
                message = "库存日志添加失败：" + ex.Message,
                inventoryLogId = 0
            };
        }

        return result;
    }

    /// <summary>
    /// 获取库存列表
    /// </summary>
    /// <param name="warehouseId">仓库ID</param>
    /// <param name="productId">产品ID</param>
    /// <param name="shelfId">货架ID</param>
    /// <param name="batchNumber">批次号</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页记录数</param>
    /// <returns>返回结果对象，包含成功标志、消息、总记录数和数据列表</returns>
    public static object GetInventoryList(int warehouseId = 0, int productId = 0, int shelfId = 0, string batchNumber = null, int pageIndex = 1, int pageSize = 10)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = "",
            total = 0,
            data = new List<object>()
        };

        try
        {
            // 构建查询条件
            string whereClause = "1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (warehouseId > 0)
            {
                whereClause += " AND i.WarehouseID = @WarehouseID";
                parameters.Add(new SqlParameter("@WarehouseID", warehouseId));
            }

            if (productId > 0)
            {
                whereClause += " AND i.ProductID = @ProductID";
                parameters.Add(new SqlParameter("@ProductID", productId));
            }

            if (shelfId > 0)
            {
                whereClause += " AND i.ShelfID = @ShelfID";
                parameters.Add(new SqlParameter("@ShelfID", shelfId));
            }

            if (!string.IsNullOrEmpty(batchNumber))
            {
                whereClause += " AND i.BatchNumber = @BatchNumber";
                parameters.Add(new SqlParameter("@BatchNumber", batchNumber));
            }

            // 处理分页参数
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            int startRow = (pageIndex - 1) * pageSize + 1;
            int endRow = pageIndex * pageSize;

            parameters.Add(new SqlParameter("@StartRow", startRow));
            parameters.Add(new SqlParameter("@EndRow", endRow));

            // 构建SQL查询语句
            string sql = $@"WITH OrderedInventory AS (
                            SELECT 
                                i.InventoryID, 
                                i.WarehouseID, 
                                w.WarehouseName,
                                i.ProductID, 
                                p.ProductName, 
                                p.ProductCode,
                                i.ShelfID, 
                                s.ShelfName,
                                i.Quantity, 
                                i.BatchNumber, 
                                i.ExpiryDate,
                                ROW_NUMBER() OVER (ORDER BY i.InventoryID) AS RowNum
                            FROM 
                                Inventory i
                                INNER JOIN Products p ON i.ProductID = p.ProductID
                                INNER JOIN Warehouses w ON i.WarehouseID = w.WarehouseID
                                INNER JOIN Shelves s ON i.ShelfID = s.ShelfID
                            WHERE 
                                {whereClause}
                        )
                        SELECT * FROM OrderedInventory
                        WHERE RowNum BETWEEN @StartRow AND @EndRow";

            // 获取总记录数
            string countSql = $@"SELECT COUNT(*) 
                                FROM Inventory i
                                INNER JOIN Products p ON i.ProductID = p.ProductID
                                INNER JOIN Warehouses w ON i.WarehouseID = w.WarehouseID
                                INNER JOIN Shelves s ON i.ShelfID = s.ShelfID
                                WHERE {whereClause}";

            int totalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(countSql, parameters.ToArray()));

            // 查询数据列表
            DataTable dt = SqlHelper.ExecuteDataTable(sql, parameters.ToArray());
            List<object> dataList = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                dataList.Add(new
                {
                    inventoryId = Convert.ToInt32(row["InventoryID"]),
                    warehouseId = Convert.ToInt32(row["WarehouseID"]),
                    warehouseName = row["WarehouseName"].ToString(),
                    productId = Convert.ToInt32(row["ProductID"]),
                    productName = row["ProductName"].ToString(),
                    productCode = row["ProductCode"].ToString(),
                    shelfId = Convert.ToInt32(row["ShelfID"]),
                    shelfName = row["ShelfName"].ToString(),
                    quantity = Convert.ToDecimal(row["Quantity"]),
                    batchNumber = row["BatchNumber"] == DBNull.Value ? "" : row["BatchNumber"].ToString(),
                    expiryDate = row["ExpiryDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ExpiryDate"])
                });
            }

            // 返回结果
            result = new
            {
                success = true,
                message = "查询成功",
                total = totalCount,
                data = dataList
            };
        }
        catch (Exception ex)
        {
            // 异常处理
            result = new
            {
                success = false,
                message = "查询库存失败：" + ex.Message,
                total = 0,
                data = new List<object>()
            };
        }

        return result;
    }

    #endregion
}

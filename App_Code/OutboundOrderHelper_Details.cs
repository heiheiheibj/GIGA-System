using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// OutboundOrderHelper_Details 的摘要说明
/// </summary>
public partial class OutboundOrderHelper
{
    #region 出库单明细相关方法

    /// <summary>
    /// 保存出库单明细
    /// </summary>
    /// <param name="outboundOrderDetailId">出库单明细ID，新增时为0</param>
    /// <param name="outboundOrderId">出库单ID</param>
    /// <param name="productId">产品ID</param>
    /// <param name="quantity">数量</param>
    /// <param name="unitPrice">单价</param>
    /// <param name="batchNumber">批次号</param>
    /// <param name="expiryDate">过期日期</param>
    /// <param name="remark">备注</param>
    /// <returns>返回结果对象，包含成功标志、消息、出库单明细ID</returns>
    public static object SaveOutboundOrderDetail(int outboundOrderDetailId, int outboundOrderId, int productId, decimal quantity, decimal unitPrice, string batchNumber, DateTime? expiryDate, string remark)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = "",
            outboundOrderDetailId = 0
        };

        // 参数验证
        if (outboundOrderId <= 0)
        {
            return new { success = false, message = "出库单ID无效", outboundOrderDetailId = 0 };
        }

        if (productId <= 0)
        {
            return new { success = false, message = "产品ID无效", outboundOrderDetailId = 0 };
        }

        if (quantity <= 0)
        {
            return new { success = false, message = "数量必须大于0", outboundOrderDetailId = 0 };
        }

        // 检查出库单是否已审核
        bool isApproved = CheckOutboundOrderApproved(outboundOrderId);
        if (isApproved)
        {
            return new { success = false, message = "出库单已审核，不能修改明细", outboundOrderDetailId = 0 };
        }

        try
        {
            // 创建SQL参数列表
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OutboundOrderDetailID", outboundOrderDetailId));
            parameters.Add(new SqlParameter("@OutboundOrderID", outboundOrderId));
            parameters.Add(new SqlParameter("@ProductID", productId));
            parameters.Add(new SqlParameter("@Quantity", quantity));
            parameters.Add(new SqlParameter("@UnitPrice", unitPrice));
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
            SqlParameter outputParam = new SqlParameter("@NewOutboundOrderDetailID", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            parameters.Add(outputParam);

            // 执行存储过程
            int rowsAffected = SqlHelper.ExecuteNonQuery("sp_OutboundOrderDetails_Save", parameters.ToArray());

            // 获取输出参数值
            int newOutboundOrderDetailId = Convert.ToInt32(outputParam.Value);

            // 返回结果
            result = new
            {
                success = true,
                message = "保存出库单明细成功",
                outboundOrderDetailId = newOutboundOrderDetailId
            };
        }
        catch (Exception ex)
        {
            // 异常处理
            result = new
            {
                success = false,
                message = "保存出库单明细失败：" + ex.Message,
                outboundOrderDetailId = 0
            };
        }

        return result;
    }

    /// <summary>
    /// 删除出库单明细
    /// </summary>
    /// <param name="outboundOrderDetailId">出库单明细ID</param>
    /// <returns>返回结果对象，包含成功标志和消息</returns>
    public static object DeleteOutboundOrderDetail(int outboundOrderDetailId)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = ""
        };

        // 参数验证
        if (outboundOrderDetailId <= 0)
        {
            return new { success = false, message = "出库单明细ID无效" };
        }

        try
        {
            // 获取出库单明细ID对应的出库单ID
            int outboundOrderId = GetOutboundOrderIdByDetailId(outboundOrderDetailId);
            if (outboundOrderId <= 0)
            {
                return new { success = false, message = "未找到要删除的明细记录" };
            }

            // 检查出库单是否已审核
            bool isApproved = CheckOutboundOrderApproved(outboundOrderId);
            if (isApproved)
            {
                return new { success = false, message = "出库单已审核，不能删除明细" };
            }

            // 创建SQL参数
            SqlParameter parameter = new SqlParameter("@OutboundOrderDetailID", outboundOrderDetailId);

            // 执行删除操作
            string sql = "DELETE FROM OutboundOrderDetails WHERE OutboundOrderDetailID = @OutboundOrderDetailID";
            int rowsAffected = SqlHelper.ExecuteNonQuery(sql, parameter);

            // 返回结果
            if (rowsAffected > 0)
            {
                result = new
                {
                    success = true,
                    message = "删除成功"
                };
            }
            else
            {
                result = new
                {
                    success = false,
                    message = "未找到要删除的明细记录"
                };
            }
        }
        catch (Exception ex)
        {
            // 异常处理
            result = new
            {
                success = false,
                message = "删除出库单明细失败：" + ex.Message
            };
        }

        return result;
    }

    /// <summary>
    /// 获取出库单明细列表
    /// </summary>
    /// <param name="outboundOrderId">出库单ID</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页记录数</param>
    /// <returns>返回结果对象，包含成功标志、消息、总记录数和数据列表</returns>
    public static object GetOutboundOrderDetailList(int outboundOrderId, int pageIndex = 1, int pageSize = 10)
    {
        // 初始化返回结果对象
        object result = new
        {
            success = false,
            message = "",
            total = 0,
            data = new List<object>()
        };

        // 参数验证
        if (outboundOrderId <= 0)
        {
            return new { success = false, message = "出库单ID无效", total = 0, data = new List<object>() };
        }

        try
        {
            // 处理分页参数
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;
            int startRow = (pageIndex - 1) * pageSize + 1;
            int endRow = pageIndex * pageSize;

            // 构建SQL查询语句
            string sql = @"WITH OrderedDetails AS (
                            SELECT 
                                d.OutboundOrderDetailID, 
                                d.OutboundOrderID, 
                                d.ProductID, 
                                p.ProductName, 
                                p.ProductCode, 
                                d.Quantity, 
                                d.UnitPrice, 
                                d.BatchNumber, 
                                d.ExpiryDate, 
                                d.Remark,
                                ROW_NUMBER() OVER (ORDER BY d.OutboundOrderDetailID) AS RowNum
                            FROM 
                                OutboundOrderDetails d
                                INNER JOIN Products p ON d.ProductID = p.ProductID
                            WHERE 
                                d.OutboundOrderID = @OutboundOrderID
                        )
                        SELECT * FROM OrderedDetails
                        WHERE RowNum BETWEEN @StartRow AND @EndRow";

            // 创建SQL参数
            SqlParameter[] parameters = {
                new SqlParameter("@OutboundOrderID", outboundOrderId),
                new SqlParameter("@StartRow", startRow),
                new SqlParameter("@EndRow", endRow)
            };

            // 获取总记录数
            string countSql = "SELECT COUNT(*) FROM OutboundOrderDetails WHERE OutboundOrderID = @OutboundOrderID";
            int totalCount = Convert.ToInt32(SqlHelper.ExecuteScalar(countSql, new SqlParameter("@OutboundOrderID", outboundOrderId)));

            // 查询数据列表
            DataTable dt = SqlHelper.ExecuteDataTable(sql, parameters);
            List<object> dataList = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                dataList.Add(new
                {
                    outboundOrderDetailId = Convert.ToInt32(row["OutboundOrderDetailID"]),
                    outboundOrderId = Convert.ToInt32(row["OutboundOrderID"]),
                    productId = Convert.ToInt32(row["ProductID"]),
                    productName = row["ProductName"].ToString(),
                    productCode = row["ProductCode"].ToString(),
                    quantity = Convert.ToDecimal(row["Quantity"]),
                    unitPrice = Convert.ToDecimal(row["UnitPrice"]),
                    batchNumber = row["BatchNumber"] == DBNull.Value ? "" : row["BatchNumber"].ToString(),
                    expiryDate = row["ExpiryDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ExpiryDate"]),
                    remark = row["Remark"] == DBNull.Value ? "" : row["Remark"].ToString()
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
                message = "查询出库单明细失败：" + ex.Message,
                total = 0,
                data = new List<object>()
            };
        }

        return result;
    }

    // 辅助方法：根据明细ID获取出库单ID
    private static int GetOutboundOrderIdByDetailId(int outboundOrderDetailId)
    {
        string sql = "SELECT OutboundOrderID FROM OutboundOrderDetails WHERE OutboundOrderDetailID = @OutboundOrderDetailID";
        SqlParameter parameter = new SqlParameter("@OutboundOrderDetailID", outboundOrderDetailId);
        object result = SqlHelper.ExecuteScalar(sql, parameter);
        return result == DBNull.Value ? 0 : Convert.ToInt32(result);
    }

    // 辅助方法：检查出库单是否已审核
    private static bool CheckOutboundOrderApproved(int outboundOrderId)
    {
        string sql = "SELECT Status FROM OutboundOrders WHERE OutboundOrderID = @OutboundOrderID";
        SqlParameter parameter = new SqlParameter("@OutboundOrderID", outboundOrderId);
        object result = SqlHelper.ExecuteScalar(sql, parameter);
        if (result == DBNull.Value) return false;
        int status = Convert.ToInt32(result);
        return status == 2; // 假设状态2表示已审核
    }

    #endregion
}

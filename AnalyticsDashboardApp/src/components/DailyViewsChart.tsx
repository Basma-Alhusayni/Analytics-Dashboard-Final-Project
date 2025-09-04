import React from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import type { DailyViewsDto } from "../types";

interface DailyViewsChartProps {
  data: DailyViewsDto[];
}

const DailyViewsChart: React.FC<DailyViewsChartProps> = ({ data }) => {
  const chartData = data.map((item) => ({
    date: new Date(item.date).toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
    }),
    views: item.views,
  }));

  return (
    <ResponsiveContainer width="100%" height={300}>
      <BarChart data={chartData}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="date" />
        <YAxis />
        <Tooltip />
        <Bar dataKey="views" fill="#3498db" />
      </BarChart>
    </ResponsiveContainer>
  );
};

export default DailyViewsChart;

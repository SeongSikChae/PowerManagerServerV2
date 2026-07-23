import * as React from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

interface PlaceholderPageProps {
  title: string;
  description?: string;
}

export default function PlaceholderPage({
  title,
  description = "추후 기능이 추가될 예정입니다.",
}: PlaceholderPageProps) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-extrabold text-sb-dark">{title}</h1>
        <p className="mt-1 text-sm text-muted-foreground">{description}</p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>준비 중</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">
            이 메뉴는 현재 깡통 페이지입니다. 이후 API 연동 및 화면을 구현할 수 있습니다.
          </p>
        </CardContent>
      </Card>
    </div>
  );
}

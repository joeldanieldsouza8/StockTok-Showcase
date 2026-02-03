import { auth0 } from "@/lib/auth0";
import { NextRequest, NextResponse } from "next/server";

const API_GATEWAY_URL = process.env.API_GATEWAY_URL;

export async function GET(request: NextRequest) {
  try {
    const { token } = await auth0.getAccessToken();
    const searchParams = request.nextUrl.searchParams;
    const count = searchParams.get("count") || "3";

    const response = await fetch(
      `${API_GATEWAY_URL}/api/watchlists/top-tickers?count=${count}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const data = await response.json();

    if (!response.ok) {
      return NextResponse.json(
        { error: data.error || "Failed to fetch top tickers" },
        { status: response.status }
      );
    }

    return NextResponse.json(data);
  } catch (error: any) {
    console.error("Error fetching top tickers:", error);
    return NextResponse.json(
      { error: "Failed to fetch top tickers", details: error.message },
      { status: 500 }
    );
  }
}


export async function Login(userName, password, rememberMe) {
    const respon = await fetch(`Account/Login`, {
        headers: {
            "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify({ userName, password, rememberMe })
    });
    return await respon.json();
}
import { createTheme } from "@mui/material/styles";

const theme = createTheme({
    palette: {
        mode: "light",
        primary: {
            main: "#0d47a1", // Navy Blue
            light: "#5472d3",
            dark: "#002171",
            contrastText: "#ffffff",
        },
        secondary: {
            main: "#1976d2", // Lighter blue accent
        },
        background: {
            default: "#f4f6f8", // Light gray background for the app
            paper: "#ffffff",
        },
        text: {
            primary: "#1c2025",
            secondary: "#374151",
        },
    },
    typography: {
        fontFamily: [
            '"Inter"',
            '"Roboto"',
            '"Helvetica"',
            '"Arial"',
            "sans-serif",
        ].join(","),
        h1: { fontWeight: 600, fontSize: "2.5rem" },
        h2: { fontWeight: 600, fontSize: "2rem" },
        h3: { fontWeight: 600, fontSize: "1.75rem" },
        h4: { fontWeight: 600, fontSize: "1.5rem" },
        h5: { fontWeight: 500, fontSize: "1.25rem" },
        h6: { fontWeight: 500, fontSize: "1rem" },
        button: { textTransform: "none", fontWeight: 600 },
    },
    components: {
        MuiButton: {
            styleOverrides: {
                root: {
                    borderRadius: 8,
                    boxShadow: "none",
                    "&:hover": {
                        boxShadow: "none",
                    },
                },
                contained: {
                    "&:hover": {
                        backgroundColor: "#002171", // Darker navy on hover
                    },
                },
            },
        },
        MuiAppBar: {
            styleOverrides: {
                root: {
                    backgroundColor: "#0d47a1", // Navy Blue
                    color: "#ffffff",
                    boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.1)",
                },
            },
        },
        MuiCard: {
            styleOverrides: {
                root: {
                    borderRadius: 12,
                    boxShadow: "0px 4px 12px rgba(0, 0, 0, 0.05)",
                },
            },
        },
        MuiPaper: {
            styleOverrides: {
                rounded: {
                    borderRadius: 12,
                },
                elevation1: {
                    boxShadow: "0px 2px 8px rgba(0, 0, 0, 0.05)",
                },
            },
        },
        MuiTextField: {
            defaultProps: {
                variant: "outlined",
                size: "small",
            },
            styleOverrides: {
                root: {
                    "& .MuiOutlinedInput-root": {
                        borderRadius: 8,
                    },
                },
            },
        },
    },
});

export default theme;

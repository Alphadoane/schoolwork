/*
 * Question 1 – Booker University Library Book Management System
 * ---------------------------------------------------------------
 * Class  : Book
 * Database: books.dat (binary flat file)
 *
 * Operations:
 *   (i)  Insert a new book record into the database
 *   (ii) Display a list of all books in the database
 */

#include <iostream>
#include <fstream>
#include <iomanip>
#include <string>
#include <limits>

using namespace std;

// ─────────────────────────────────────────────
//  Book Class
// ─────────────────────────────────────────────
class Book {
private:
    char author[50];
    char title[100];
    char book_number[20];
    double price;
    int   copies;

public:
    // ── Setters ──────────────────────────────
    void setAuthor(const string& a)      { strncpy(author,      a.c_str(), 49);  author[49]      = '\0'; }
    void setTitle(const string& t)       { strncpy(title,       t.c_str(), 99);  title[99]       = '\0'; }
    void setBookNumber(const string& bn) { strncpy(book_number, bn.c_str(), 19); book_number[19] = '\0'; }
    void setPrice(double p)              { price  = p; }
    void setCopies(int c)                { copies = c; }

    // ── Getters ──────────────────────────────
    string getAuthor()     const { return string(author);      }
    string getTitle()      const { return string(title);       }
    string getBookNumber() const { return string(book_number); }
    double getPrice()      const { return price;  }
    int    getCopies()     const { return copies; }

    // ── (i) Insert a new book record ─────────
    void insert() {
        string a, t, bn;
        double p;
        int    c;

        cout << "\n=== Add New Book ===\n";
        cout << "  Author      : "; cin.ignore(); getline(cin, a);
        cout << "  Title       : ";               getline(cin, t);
        cout << "  Book Number : ";               getline(cin, bn);
        cout << "  Price (KES) : "; cin >> p;
        cout << "  Copies      : "; cin >> c;

        setAuthor(a);
        setTitle(t);
        setBookNumber(bn);
        setPrice(p);
        setCopies(c);

        // Append record to binary file
        ofstream outFile("books.dat", ios::binary | ios::app);
        if (!outFile) {
            cerr << "  [ERROR] Could not open books.dat for writing.\n";
            return;
        }
        outFile.write(reinterpret_cast<char*>(this), sizeof(Book));
        outFile.close();

        cout << "  [OK] Book record saved successfully.\n";
    }

    // ── (ii) Display all book records ────────
    static void display() {
        ifstream inFile("books.dat", ios::binary);
        if (!inFile) {
            cout << "\n  [INFO] No records found. Database is empty.\n";
            return;
        }

        Book b;
        int  count = 0;

        cout << "\n=== Booker University Library – Book Inventory ===\n";
        cout << left
             << setw(12) << "Book No."
             << setw(40) << "Title"
             << setw(25) << "Author"
             << setw(12) << "Price(KES)"
             << setw(8)  << "Copies"
             << "\n";
        cout << string(97, '-') << "\n";

        while (inFile.read(reinterpret_cast<char*>(&b), sizeof(Book))) {
            cout << left
                 << setw(12) << b.getBookNumber()
                 << setw(40) << b.getTitle()
                 << setw(25) << b.getAuthor()
                 << setw(12) << fixed << setprecision(2) << b.getPrice()
                 << setw(8)  << b.getCopies()
                 << "\n";
            ++count;
        }
        inFile.close();

        if (count == 0)
            cout << "  [INFO] No records found.\n";
        else
            cout << string(97, '-') << "\n"
                 << "  Total books: " << count << "\n";
    }
};

// ─────────────────────────────────────────────
//  Main Function
// ─────────────────────────────────────────────
int main() {
    int choice;

    cout << "╔══════════════════════════════════════╗\n";
    cout << "║  Booker University Library System    ║\n";
    cout << "╚══════════════════════════════════════╝\n";

    do {
        cout << "\n--- MENU ---\n"
             << "  1. Add a new book\n"
             << "  2. Display all books\n"
             << "  0. Exit\n"
             << "Enter choice: ";
        cin >> choice;

        // Clear any bad input
        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            choice = -1;
        }

        switch (choice) {
            case 1: {
                Book b;
                b.insert();
                break;
            }
            case 2:
                Book::display();
                break;
            case 0:
                cout << "Goodbye!\n";
                break;
            default:
                cout << "  Invalid option. Try again.\n";
        }
    } while (choice != 0);

    return 0;
}

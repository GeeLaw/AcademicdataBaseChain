# Academic dataBase Chain (学表链)

Humourously known as Academic Bitch Chain (学婊链).

This is a hackathon project during 28—29 April 2018 X-lab x DoraHacks Hackathon at Tsinghua Science Park in Beijing. Our team consisted of six members: Gee Law, Ting Fung Lau, Wangbin Sun, Congrong Ma, Jingyi Wang, Chelsea (Ziche) Liu. The project aims to provide a blockchain solution to transcript issuance, publishing and sharing.

In the chain, entities are divided into the following categories:

- Bureau of education, whose address is appointed by the time of deployment;
- Schools, whose addresses are nominated and unnominated by BoE;
- Others, the rest.

On the chain, schools are allowed to publish transcript records, which displays school address and transcript identifier in clear, and which stores transcript encrypted with a one-time symmetric key. The encryption shall be non-equivocable, which means it is infeasible to ‘decrypt’ it into another cleartext. (This is currently not enforced.) Once transcript records are issued, the school (issuer) might send the transcript identifiers and keys to the students, who can then securely check their grades and share them with others as they wish. It is also possible to introduce a mechanism to revoke a record, in case an errorneous higher grade is issued. (This is currently not implemented.)

With these available, one can add smart contracts onto the chain. For example, scholarships based on coursework excellence might be done via smart contract. In parts of the world (e.g., China), language tutoring organisations might provide an insurance to their students, allowing them to take more hours of courses (or to acquire compensation in another form such as refund of tuition fees) if they did not make it in language-related standardised tests (TOEFL and GRE among others). These insurances contracts can also be automated. For example, one might provide infrastructure for insurance offer issuance, acceptation and execution.

We were using INKChain BaaS (Blockchain-as-a-Service) by Ziggurat Technologies when the product was in early alpha phase, and there were many resitrctions and some inconvenience in programming smart contracts. For example, a smart contract has not its own address, and during the processing, only the one who activates a contract can be made to pay. This makes exercising of insurance contract difficult if the underwriter defaults. This can be worked around by keeping a transient record of debt and enforce one to clear up the debts before each activation of smart contract. The other glitch in their chain is that the permissions are controlled by self-discipline (APIs are in prototype phase). Cryptographic solutions are expected for the generally available product.

During the 24 hours, we implemented a simple contract system for storing and querying encrypted transcripts and the framework of what we would need to implement insurance contracts. We also included a client application with Windows Presentation Foundation. The project won the first place among the seven fascinating ones.

The code in commit `95dca94856f9070319bc8ab3630b09628b4d8859` (the commit before `README.md` and `LICENSE.md` are added) is a full restoration of the state of the project before the awarding, except that the API keys and entities’ addresses are removed. Since we had to squash serveral commits into one, the author information was lost. Gee, Ting Fung and Chelsea contributed to coding, and Wangbin, Congrong and Jingyi contributed to slides. Everyone contribued to presentation.

**Anecdote** For those who wonder why ‘academic bitch’ (学婊), the idea comes from the common phenomenon that high-tier students often feel anixous about their grades before they are available, constantly saying ‘I must have failed the exame’ and it often turns out that they have high grades. The insurance is a joke on them — we should sell them insurances on the grades, and make hugh profit out of them.

The group name was ‘六个学神’ (six super learner), and the project was promoted as ‘集成学历、成绩和教育服务的一条链’ (a chain integrating educational experience, grades and educational services).

You can read my blog entry on this hackathon [here](https://geelaw.blog/entries/2018-04-dorahacks-beijing/) (in Chinese).
